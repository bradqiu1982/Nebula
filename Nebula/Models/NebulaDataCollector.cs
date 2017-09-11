﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Nebula.Models
{

    public class NativeMethods : IDisposable
    {

        // obtains user token  

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]

        static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,

            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);



        // closes open handes returned by LogonUser  

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        extern static bool CloseHandle(IntPtr handle);
        [DllImport("Advapi32.DLL")]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
        [DllImport("Advapi32.DLL")]
        static extern bool RevertToSelf();
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_NEWCREDENTIALS = 2;

        private bool disposed;

        public NativeMethods(string sUsername, string sDomain, string sPassword)
        {

            // initialize tokens  

            IntPtr pExistingTokenHandle = new IntPtr(0);
            IntPtr pDuplicateTokenHandle = new IntPtr(0);
            try
            {
                // get handle to token  
                bool bImpersonated = LogonUser(sUsername, sDomain, sPassword,

                    LOGON32_LOGON_NEWCREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref pExistingTokenHandle);
                if (true == bImpersonated)
                {

                    if (!ImpersonateLoggedOnUser(pExistingTokenHandle))
                    {
                        int nErrorCode = Marshal.GetLastWin32Error();
                        throw new Exception("ImpersonateLoggedOnUser error;Code=" + nErrorCode);
                    }
                }
                else
                {
                    int nErrorCode = Marshal.GetLastWin32Error();
                    throw new Exception("LogonUser error;Code=" + nErrorCode);
                }

            }

            finally
            {
                // close handle(s)  
                if (pExistingTokenHandle != IntPtr.Zero)
                    CloseHandle(pExistingTokenHandle);
                if (pDuplicateTokenHandle != IntPtr.Zero)
                    CloseHandle(pDuplicateTokenHandle);
            }

        }

        protected virtual void Dispose(bool disposing)
        {

            if (!disposed)
            {
                RevertToSelf();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
    

    public class NebulaAGILEDOWNLOADTYPE
    {
        public static string ATTACH = "ATTACH";
        public static string ATTACHNAME = "ATTACHNAME";
        public static string WORKFLOW = "WORKFLOW";
    }

    public class ECOWorkFlowRAWData
    {
        public ECOWorkFlowRAWData()
        {
            StatusCode = "";
            WorkFlow = "";
            WorkFlowStatus = "";
            Action = "";
            Reqd = "";
            Reviewer = "";
            SignoffUser = "";
            StatusChangedBy = "";
            LocalTime = "";
            SignoffComment = "";
            SignoffDuration = "";
        }

        public string StatusCode { set; get; }
        public string WorkFlow { set; get; }
        public string WorkFlowStatus { set; get; }
        public string Action { set; get; }
        public string Reqd { set; get; }
        public string Reviewer { set; get; }
        public string SignoffUser { set; get; }
        public string StatusChangedBy { set; get; }
        public string LocalTime { set; get; }
        public string SignoffComment { set; get; }
        public string SignoffDuration { set; get; }
    }

    public class ECOWorkFlowInfo
    {
        public ECOWorkFlowInfo()
        {
            CurrentProcess = "";
            ECOTRApprover = "";
            ECOMDApprover = "";
            CApproveHoldDate = "";
            ECOCompleteDate = "";
            WorkFlowType = "";
        }

        public string CurrentProcess { set; get; }
        public string ECOTRApprover { set; get; }
        public string ECOMDApprover { set; get; }
        public string CApproveHoldDate { set; get; }
        public string ECOCompleteDate { set; get; }
        public string WorkFlowType { set; get; }
    }

    public class NebulaDataCollector
    {

        private static bool FileExist(Controller ctrl,string filename)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return File.Exists(filename);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static void FileCopy(Controller ctrl,string src, string des, bool overwrite,bool checklocal = false)
        {
            try
            {


                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    if (checklocal)
                    {
                        if (File.Exists(des))
                        {
                            return;
                        }
                    }

                    File.Copy(src,des,overwrite);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static bool DirectoryExists(Controller ctrl, string dirname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return Directory.Exists(dirname);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static List<string> DirectoryEnumerateFiles(Controller ctrl, string dirname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    var ret = new List<string>();
                    ret.AddRange(Directory.EnumerateFiles(dirname));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<List<string>> RetrieveDataFromExcel(Controller ctrl, string filename,string sheetname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return ExcelReader.RetrieveDataFromExcel(filename, sheetname);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static List<string> DirectoryEnumerateDirs(Controller ctrl, string dirname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    var ret = new List<string>();
                    ret.AddRange(Directory.EnumerateDirectories(dirname));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void DownloadAgile(List<string> ecolist, Controller ctrl,string downloadtype)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var AGILEURL = syscfgdict["AGILEURL"];
            var LOCALSITEPORT = syscfgdict["LOCALSITEPORT"];
            var SAVELOCATION = syscfgdict["SAVELOCATION"];

            var ecostr = string.Empty;
            foreach (var eco in ecolist)
            {
                ecostr = ecostr + " " + eco+" ";
            }

            var args = downloadtype+" " + AGILEURL + " " + LOCALSITEPORT + " " + SAVELOCATION + " " + ecostr;

            using (Process myprocess = new Process())
            {
                myprocess.StartInfo.FileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, @"Scripts\agiledownloadwraper\AgileDownload.exe").Replace("\\", "/");
                myprocess.StartInfo.Arguments = args;
                myprocess.StartInfo.CreateNoWindow = true;
                myprocess.Start();
            }
        }

        public static ECOWorkFlowInfo RetrieveAgileWorkFlowData(string ECONUM,Controller ctrl)
        {
            var rawdata = new List<ECOWorkFlowRAWData>();

            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var dir = syscfgdict["SAVELOCATION"] + "\\" + ECONUM;
            var workflowfile = dir + "\\" + ECONUM + "_WorkFlowTable.csv";
            if (FileExist(ctrl,workflowfile))
            {
                var data = RetrieveDataFromExcel(ctrl,workflowfile, null);
                foreach (var line in data)
                {
                    var tempdata = new ECOWorkFlowRAWData();
                    tempdata.StatusCode = line[0];
                    tempdata.WorkFlow = line[1];
                    tempdata.WorkFlowStatus = line[2];
                    tempdata.Action = line[3];
                    tempdata.Reqd = line[4];
                    tempdata.Reviewer = line[5];
                    tempdata.SignoffUser = line[6];
                    tempdata.StatusChangedBy = line[7];
                    tempdata.LocalTime = line[8];
                    tempdata.SignoffComment = line[9];
                    tempdata.SignoffDuration = line[10];
                    rawdata.Add(tempdata);
                }
            }

            var ret = new ECOWorkFlowInfo();
            bool bcompletedate = false;

            foreach (var line in rawdata)
            {
                if (bcompletedate)
                {
                    ret.ECOCompleteDate = line.LocalTime;
                    bcompletedate = false;
                }

                if (string.Compare(line.StatusCode, "Current Process", true) == 0)
                {
                    ret.CurrentProcess = line.WorkFlowStatus;
                    if (line.WorkFlow.ToUpper().Contains(NebulaFlowInfo.Revise.ToUpper()))
                    {
                        ret.WorkFlowType = NebulaFlowInfo.Revise;
                    }
                    else
                    {
                        ret.WorkFlowType = NebulaFlowInfo.Default;
                    }

                    if (string.Compare(ret.CurrentProcess, "Completed") == 0)
                    {
                        bcompletedate = true;
                    }
                }

                if (string.Compare(line.WorkFlowStatus, "Technical Review", true) == 0
                    && string.Compare(line.Reqd, "Yes", true) == 0
                    && !string.IsNullOrEmpty(line.Reviewer)
                    && !ret.ECOTRApprover.Contains(line.Reviewer))
                {
                    ret.ECOTRApprover = ret.ECOTRApprover + line.Reviewer + ";";
                }

                if (string.Compare(line.WorkFlowStatus, "Material Disposition", true) == 0
                    && string.Compare(line.Reqd, "Yes", true) == 0
                    && !string.IsNullOrEmpty(line.Reviewer)
                    && !ret.ECOMDApprover.Contains(line.Reviewer))
                {
                    ret.ECOMDApprover = ret.ECOMDApprover + line.Reviewer + ";";
                }

                if (line.WorkFlowStatus.ToUpper().Contains("CCB")
                    && string.IsNullOrEmpty(ret.CApproveHoldDate)
                    && string.IsNullOrEmpty(line.Reqd)
                    && string.IsNullOrEmpty(line.Reviewer)
                    && !string.IsNullOrEmpty(line.StatusChangedBy)
                    && !string.IsNullOrEmpty(line.LocalTime))
                {
                    ret.CApproveHoldDate = line.LocalTime;
                }

            }

            return ret;
        }

        public static void UpdateECOWeeklyUpdate(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            var desfolder = syscfgdict["WEEKLYUPDATE"];
            var sheetname = syscfgdict["MINIPIPSHEETNAME"];
            try
            {
                if (!DirectoryExists(ctrl,imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                var pendinghistory = new List<ECOPendingUpdate>();

                if (DirectoryExists(ctrl,desfolder))
                {
                    var fds = DirectoryEnumerateFiles(ctrl,desfolder);
                    foreach (var fd in fds)
                    {
                        try
                        {
                            var timestrs = Path.GetFileNameWithoutExtension(fd).Split(new string[] {" "},StringSplitOptions.RemoveEmptyEntries);
                            var timestamp = timestrs[timestrs.Length - 1];
                            if (timestamp.Length != 8)
                            {
                                continue;
                            }

                            var fn = imgdir + Path.GetFileName(fd);
                            FileCopy(ctrl,fd, fn, true,true);
                            var data = RetrieveDataFromExcel(ctrl,fn, sheetname);
                            foreach (var line in data)
                            {
                                if (string.Compare(line[2], baseinfo.PNDesc, true) == 0
                                    && string.Compare(DateTime.Parse(line[12]).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                                {
                                    var tempinfo = new ECOPendingUpdate();
                                    tempinfo.CardKey = cardkey;
                                    tempinfo.History = line[85];
                                    tempinfo.UpdateTime = timestamp.Substring(0,4)+"-"+timestamp.Substring(4,2)+"-"+timestamp.Substring(6,2)+" 07:30:00";
                                    pendinghistory.Add(tempinfo);
                                }//end if
                            }//end foreach
                        }
                        catch (Exception ex) { }
                    }//end foreach

                    NebulaVM.UpdateHistoryInfo(pendinghistory, cardkey);

                    var spcard = NebulaVM.RetrieveCard(cardkey);
                    if (spcard.Count > 0
                        && !string.IsNullOrEmpty(baseinfo.ECONum)
                        && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                    {
                        NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                    }

                }
            }
            catch (Exception ex) { }

        }

        private static string ConvertToDate(string datestr)
        {
            if (string.IsNullOrEmpty(datestr))
            {
                return string.Empty;
            }
            try
            {
                return DateTime.Parse(datestr).ToString();
            }
            catch (Exception ex) { return string.Empty; }
        }

        private static void updateecolist(List<List<string>> data, Controller ctrl, string localdir, string urlfolder)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var baseinfos = ECOBaseInfo.RetrieveAllExistECOBaseInfo();

            foreach (var line in data)
            {
                if (!string.IsNullOrEmpty(line[2])
                    && !string.IsNullOrEmpty(line[12])
                    && !string.IsNullOrEmpty(line[23])
                    && (string.Compare(line[23], "C", true) == 0
                    || string.Compare(line[23], "W", true) == 0)
                    && string.Compare(line[81], "canceled", true) != 0)
                {
                    var initialmini = string.Empty;
                    try
                    {
                        initialmini = DateTime.Parse(line[12]).ToString();
                    }
                    catch (Exception ex)
                    { initialmini = string.Empty; }

                    if (string.IsNullOrEmpty(initialmini)
                        || DateTime.Parse(initialmini) < DateTime.Parse("2017-01-01 01:00:00"))
                    {
                        continue;
                    }

                    var baseinfo = new ECOBaseInfo();
                    baseinfo.PNDesc = line[2];
                    baseinfo.Customer = line[5];
                    baseinfo.Complex = line[6];
                    baseinfo.RSM = line[7];
                    baseinfo.PE = line[8];
                    baseinfo.RiskBuild = line[9];

                    baseinfo.InitRevison = ConvertToDate(line[12]);
                    baseinfo.FinalRevison = ConvertToDate(line[13]);
                    baseinfo.TLAAvailable = ConvertToDate(line[14]);
                    baseinfo.OpsEntry = ConvertToDate(line[17]);
                    baseinfo.TestModification = ConvertToDate(line[18]);
                    baseinfo.ECOSubmit = ConvertToDate(line[19]);
                    baseinfo.ECOReviewSignoff = ConvertToDate(line[79]);
                    baseinfo.ECOCCBSignoff = ConvertToDate(line[80]);
                    baseinfo.ECONum = line[81];
                    baseinfo.QTRInit = line[82];

                    bool ecoexist = false;
                    foreach (var item in baseinfos)
                    {
                        if (string.Compare(item.PNDesc, baseinfo.PNDesc, true) == 0
                            && string.Compare(DateTime.Parse(item.InitRevison).ToString("yyyy-MM-dd"), DateTime.Parse(baseinfo.InitRevison).ToString("yyyy-MM-dd"), true) == 0)
                        {
                            ecoexist = true;

                            if (string.Compare(item.MiniPIPStatus, NebulaMiniPIPStatus.working) != 0)
                            {
                                break;
                            }

                            item.Customer = baseinfo.Customer;
                            item.Complex = baseinfo.Complex;
                            item.RSM = baseinfo.RSM;
                            item.PE = baseinfo.PE;
                            item.RiskBuild = baseinfo.RiskBuild;

                            item.FinalRevison = baseinfo.FinalRevison;
                            item.TLAAvailable = baseinfo.TLAAvailable;
                            item.OpsEntry = baseinfo.OpsEntry;
                            item.TestModification = baseinfo.TestModification;
                            item.ECOSubmit = baseinfo.ECOSubmit;
                            item.ECOReviewSignoff = baseinfo.ECOReviewSignoff;
                            item.ECOCCBSignoff = baseinfo.ECOCCBSignoff;
                            item.ECONum = baseinfo.ECONum;
                            item.QTRInit = baseinfo.QTRInit;
                            item.UpdateECO();

                            var pendingcard = NebulaVM.RetrieveSpecialCard(item, NebulaCardType.ECOPending);

                            if (pendingcard.Count > 0)
                            {
                                if (string.IsNullOrEmpty(item.ECONum))
                                {
                                    try
                                    {
                                        var PendingDays = "0";
                                        if (!string.IsNullOrEmpty(item.FinalRevison))
                                        {
                                            PendingDays = (DateTime.Now - DateTime.Parse(item.FinalRevison)).Days.ToString();
                                        }
                                        else
                                        {
                                            PendingDays = (DateTime.Now - DateTime.Parse(item.InitRevison)).Days.ToString();
                                        }
                                        
                                        NebulaVM.UpdateECOPendingPendingDays(pendingcard[0].CardKey, PendingDays);
                                    }
                                    catch (Exception ex) { }
                                }

                                if (!string.IsNullOrEmpty(item.ECONum)
                                && string.Compare(pendingcard[0].CardStatus, NebulaCardStatus.working) == 0)
                                {
                                    NebulaVM.UpdateCardStatus(pendingcard[0].CardKey, NebulaCardStatus.pending);
                                }

                                var allattach = NebulaVM.RetrieveCardExistedAttachment(pendingcard[0].CardKey);

                                var customerfold = new List<string>();
                                var allcustomerfolder = DirectoryEnumerateDirs(ctrl,syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                                foreach (var cf in allcustomerfolder)
                                {
                                    var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                                    var lastfd = lastlevelfd[lastlevelfd.Length - 1].Replace(" ", "").Replace("-", "").Replace("_", "").Replace("'", "").ToUpper();
                                    var baseinfocustomer = baseinfo.Customer.Replace(" ", "").Replace("-", "").Replace("_", "").Replace("'", "").ToUpper();

                                    if (lastfd.Contains(baseinfocustomer)
                                        || baseinfocustomer.Contains(lastfd))
                                    {
                                        customerfold.Add(cf);
                                    }
                                }

                                    foreach (var cf in customerfold)
                                    { 
                                        var MiniPIPDocFolder = cf + "\\" + baseinfo.PNDesc;

                                        if (DirectoryExists(ctrl,MiniPIPDocFolder))
                                        {
                                            var minidocfiles = DirectoryEnumerateFiles(ctrl,MiniPIPDocFolder);
                                            foreach (var minifile in minidocfiles)
                                            {
                                                var fn = System.IO.Path.GetFileName(minifile);
                                                fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                                                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                                                bool attachexist = false;
                                                foreach (var att in allattach)
                                                {
                                                    if (att.Contains(fn))
                                                    {
                                                        attachexist = true;
                                                        break;
                                                    }
                                                }//end foreach

                                                if (!attachexist)
                                                {
                                                    var desfile = localdir + fn;
                                                    try
                                                    {
                                                        FileCopy(ctrl,minifile, desfile, true);
                                                        var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                                        NebulaVM.StoreCardAttachment(pendingcard[0].CardKey, url);
                                                    }
                                                    catch (Exception ex) { }
                                                }
                                            }//end foreach
                                        }//try to get mini doc file
                                }

                            }//end if

                            break;
                        }
                    }

                    if (!ecoexist)
                    {
                        baseinfo.ECOKey = NebulaVM.GetUniqKey();
                        baseinfo.CreateECO();

                        var CardKey = NebulaVM.GetUniqKey();

                        if (string.IsNullOrEmpty(baseinfo.ECONum))
                        {
                            new System.Threading.ManualResetEvent(false).WaitOne(2000);
                            var currenttime = DateTime.Now;
                            currenttime = currenttime.AddMinutes(1);
                            NebulaVM.CreateCard(baseinfo.ECOKey, CardKey, NebulaCardType.ECOPending,currenttime.ToString(), NebulaCardStatus.working);

                            var PendingDays = "0";
                            if (!string.IsNullOrEmpty(baseinfo.FinalRevison))
                            {
                                PendingDays = (DateTime.Now - DateTime.Parse(baseinfo.FinalRevison)).Days.ToString();
                            }
                            else
                            {
                                PendingDays = (DateTime.Now - DateTime.Parse(baseinfo.InitRevison)).Days.ToString();
                            }

                            NebulaVM.UpdateECOPendingPendingDays(CardKey, PendingDays);
                        }
                        else
                        {
                            new System.Threading.ManualResetEvent(false).WaitOne(2000);
                            var currenttime = DateTime.Now;
                            currenttime = currenttime.AddMinutes(1);

                            NebulaVM.CreateCard(baseinfo.ECOKey, CardKey, NebulaCardType.ECOPending,currenttime.ToString(), NebulaCardStatus.pending);
                        }

                        var customerfold = new List<string>();
                        var allcustomerfolder = DirectoryEnumerateDirs(ctrl,syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                        foreach (var cf in allcustomerfolder)
                        {
                            var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            var lastfd = lastlevelfd[lastlevelfd.Length - 1].Replace(" ","").Replace("-", "").Replace("_", "").Replace("'", "").ToUpper();
                            var baseinfocustomer = baseinfo.Customer.Replace(" ", "").Replace("-", "").Replace("_", "").Replace("'", "").ToUpper();

                            if (lastfd.Contains(baseinfocustomer)
                                || baseinfocustomer.Contains(lastfd))
                            {
                                customerfold.Add(cf);
                            }
                        }

                        foreach (var cf in customerfold)
                        {
                            var MiniPIPDocFolder = cf + "\\" + baseinfo.PNDesc;
                            if (DirectoryExists(ctrl,MiniPIPDocFolder))
                            {
                                var minidocfiles = DirectoryEnumerateFiles(ctrl,MiniPIPDocFolder);
                                foreach (var minifile in minidocfiles)
                                {
                                    var fn = System.IO.Path.GetFileName(minifile);
                                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
                                    var desfile = localdir + fn;

                                    try
                                    {
                                        FileCopy(ctrl,minifile, desfile, true);
                                        var url = "/userfiles/docs/" + urlfolder + "/" + fn;
                                        NebulaVM.StoreCardAttachment(CardKey, url);
                                    }
                                    catch (Exception ex) { }

                                }//end foreach
                            }//try to get mini doc file
                        }

                    }

                }
            }
        }

        public static void SetForceECORefresh(Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            if (FileExist(ctrl,desfile))
            {
                try
                {
                    System.IO.File.Delete(desfile);
                }
                catch (Exception ex) { }
            }
        }

        public static void RefreshECOList(Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = imgdir + syscfgdict["MINIPIPECOFILENAME"];
            try
            {
                if (!DirectoryExists(ctrl,imgdir) || !FileExist(ctrl,desfile))
                {
                    if (!DirectoryExists(ctrl,imgdir))
                        Directory.CreateDirectory(imgdir);

                    var srcfile = syscfgdict["MINIPIPECOFOLDER"] + "\\" + syscfgdict["MINIPIPECOFILENAME"];

                    if (FileExist(ctrl,srcfile) && !FileExist(ctrl,desfile))
                    {
                        FileCopy(ctrl,srcfile, desfile, true);
                    }
                }

                if (FileExist(ctrl,desfile))
                {
                    var data = RetrieveDataFromExcel(ctrl,desfile, syscfgdict["MINIPIPSHEETNAME"]);
                    updateecolist(data, ctrl, imgdir, datestring);
                }
            }
            catch (Exception ex) { }
        }

        public static void RefreshECOPendingAttachInfo(Controller ctrl, string ECOKey)
        {

            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);

            foreach (var item in baseinfos)
            {
                    if (string.Compare(item.MiniPIPStatus, NebulaMiniPIPStatus.working) != 0)
                    {
                        break;
                    }

                    var pendingcard = NebulaVM.RetrieveSpecialCard(item, NebulaCardType.ECOPending);

                    if (pendingcard.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(item.ECONum)
                        && string.Compare(pendingcard[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(pendingcard[0].CardKey, NebulaCardStatus.pending);
                        }

                        var allattach = NebulaVM.RetrieveCardExistedAttachment(pendingcard[0].CardKey);

                        var customerfold = new List<string>();
                        var allcustomerfolder = DirectoryEnumerateDirs(ctrl,syscfgdict["MINIPIPCUSTOMERFOLDER"]);
                        foreach (var cf in allcustomerfolder)
                        {
                            var lastlevelfd = cf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            var lastfd = lastlevelfd[lastlevelfd.Length - 1];
                            if (lastfd.ToUpper().Contains(item.Customer.ToUpper())
                                || item.Customer.ToUpper().Contains(lastfd.ToUpper()))
                            {
                                customerfold.Add(cf);
                            }
                        }

                        foreach (var cf in customerfold)
                        {
                            var MiniPIPDocFolder = cf + "\\" + item.PNDesc;

                            if (DirectoryExists(ctrl,MiniPIPDocFolder))
                            {
                                var minidocfiles = DirectoryEnumerateFiles(ctrl,MiniPIPDocFolder);
                                foreach (var minifile in minidocfiles)
                                {
                                    var fn = System.IO.Path.GetFileName(minifile);
                                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                                    bool attachexist = false;
                                    foreach (var att in allattach)
                                    {
                                        if (att.Contains(fn))
                                        {
                                            attachexist = true;
                                            break;
                                        }
                                    }//end foreach

                                    if (!attachexist)
                                    {
                                        var desfile = imgdir + fn;
                                        try
                                        {
                                            FileCopy(ctrl,minifile, desfile, true);
                                            var url = "/userfiles/docs/" + datestring + "/" + fn;
                                            NebulaVM.StoreCardAttachment(pendingcard[0].CardKey, url);
                                        }
                                        catch (Exception ex) { }
                                    }
                                }//end foreach
                            }//try to get mini doc file
                        }

                    }//end if

                break;
            }//end foreach

        }


        private static string RMSpectialCh(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void RefreshQAEEPROMFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {

            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QAEEPROMFAI"];
            var eepromfilter = syscfgdict["QAEEPROMCHECKLISTFILTER"];

            if (DirectoryExists(ctrl,srcrootfolder))
            {
                var currentcard = NebulaVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = NebulaVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var destfolderlist = new List<string>();
                var srcfolders = DirectoryEnumerateDirs(ctrl,srcrootfolder);
                foreach (var fd in srcfolders)
                {
                    var baseinfopn = "";
                    var excelpn = "";

                    var dirs = fd.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    var expn = dirs[dirs.Length - 1].Trim();

                    if (baseinfo.PNDesc.Contains("xx"))
                    {
                        var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                        baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                        if (expn.Length > (lastidx + 2))
                        {
                            excelpn = expn.Remove(lastidx, 2);
                        }
                        else
                        {
                            excelpn = expn;
                        }
                    }
                    else
                    {
                        baseinfopn = baseinfo.PNDesc;
                        excelpn = expn;
                    }

                    if (excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                    {
                        destfolderlist.Add(fd);
                    }
                }//end foreach get folder contains PNDESC

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = DirectoryEnumerateFiles(ctrl,fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(eepromfilter.ToUpper()))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!DirectoryExists(ctrl,imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            FileCopy(ctrl,desf, attpath, true);
                            NebulaVM.StoreCardAttachment(CardKey, url);

                            var spcard = NebulaVM.RetrieveCard(CardKey);
                            if (spcard.Count > 0
                                && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.pending);
                            }
                        }
                        catch (Exception ex) { }
                    }

                }//end foreach

            }//end if

        }

        private static void RefreshQALabelFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcrootfolder = syscfgdict["QALABELFAI"];
            var labelfilter = syscfgdict["QALABELFILTER"];

            if (DirectoryExists(ctrl,srcrootfolder))
            {
                var currentcard = NebulaVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = NebulaVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var ffold = DirectoryEnumerateDirs(ctrl,srcrootfolder);

                var seconfleveldir = new List<string>();
                foreach (var ffd in ffold)
                {
                    var sfd = DirectoryEnumerateDirs(ctrl,ffd);
                    seconfleveldir.AddRange(sfd);
                }

                var destfolderlist = new List<string>();
                foreach (var desf in seconfleveldir)
                {
                    var baseinfopn = "";
                    var excelpn = "";

                    var dirs = desf.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    var expn = dirs[dirs.Length -1].Trim();

                    if (baseinfo.PNDesc.Contains("xx"))
                    {
                        var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                        baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                        if (expn.Length > (lastidx + 2))
                        {
                            excelpn = expn.Remove(lastidx, 2);
                        }
                        else
                        {
                            excelpn = expn;
                        }
                    }
                    else
                    {
                        baseinfopn = baseinfo.PNDesc;
                        excelpn = expn;
                    }

                    if (excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                    {
                        destfolderlist.Add(desf);
                    }
                }

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = DirectoryEnumerateFiles(ctrl,fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(labelfilter.ToUpper()))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!DirectoryExists(ctrl,imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            FileCopy(ctrl,desf, attpath, true);
                            NebulaVM.StoreCardAttachment(CardKey, url);

                            var spcard = NebulaVM.RetrieveCard(CardKey);
                            if (spcard.Count > 0
                                && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.pending);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }//end foreach

            }//end if

        }

        private static List<string> OAQAFilePath(ECOBaseInfo baseinfo, Controller ctrl)
        {
            var existfiledict = new Dictionary<string, bool>();

            var ret = new List<string>();
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            if (syscfgdict.ContainsKey("OAQADBSTRING"))
            {
                var conn = DBUtility.GetConnector(syscfgdict["OAQADBSTRING"]);
                if (conn != null)
                {
                    var sql = "select FileURL from View_OA_QA_FAI_File_List where ECO='<ECONum>' order by Updated_At Desc";
                    sql = sql.Replace("<ECONum>", baseinfo.ECONum);

                    var dbret = DBUtility.ExeSqlWithRes(conn, sql);
                    foreach (var line in dbret)
                    {
                        try
                        {
                            var url = Convert.ToString(line[0]);
                            var filename = Path.GetFileName(url);
                            if (!existfiledict.ContainsKey(filename))
                            {
                                existfiledict.Add(filename, true);
                                ret.Add(url);
                            }//end if
                        }
                        catch (Exception ex) { }
                    }//end foreach
                }//end if
            }//end if

            return ret;
        }

        private static void OARefreshQAEEPROMFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {

            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var eepromfilter = syscfgdict["QAEEPROMCHECKLISTFILTER"];

                var currentcard = NebulaVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = NebulaVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var destfiles = new List<string>();
                var qafiles = OAQAFilePath(baseinfo, ctrl);
                foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(eepromfilter.ToUpper()))
                        {
                            destfiles.Add(qaf);
                        }
                    }


                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");


                    var attfn = baseinfo.ECONum+"_"+baseinfo.PNDesc.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "") + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!DirectoryExists(ctrl, imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            FileCopy(ctrl, desf, attpath, true);
                            NebulaVM.StoreCardAttachment(CardKey, url);

                            var spcard = NebulaVM.RetrieveCard(CardKey);
                            if (spcard.Count > 0
                                && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.pending);
                            }
                        }
                        catch (Exception ex) { }
                    }

                }//end foreach

        }

        private static void OARefreshQALabelFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var labelfilter = syscfgdict["QALABELFILTER"];

                var currentcard = NebulaVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = NebulaVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);


                var destfiles = new List<string>();
                var qafiles = OAQAFilePath(baseinfo, ctrl);
                foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(labelfilter.ToUpper()))
                        {
                            destfiles.Add(qaf);
                        }
                    }


                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");


                var attfn = baseinfo.ECONum + "_" + baseinfo.PNDesc.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "") + "_" + fn;

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!DirectoryExists(ctrl, imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            FileCopy(ctrl, desf, attpath, true);
                            NebulaVM.StoreCardAttachment(CardKey, url);

                            var spcard = NebulaVM.RetrieveCard(CardKey);
                            if (spcard.Count > 0
                                && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.pending);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }//end foreach

        }

        public static void RefreshQAFAI(ECOBaseInfo baseinfo, string CardKey, Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            if (syscfgdict.ContainsKey("OAQASWITH")
                && string.Compare(syscfgdict["OAQASWITH"].Trim().ToUpper(), "TRUE") == 0)
            {
                OARefreshQAEEPROMFAI(baseinfo, CardKey, ctrl);
                OARefreshQALabelFAI(baseinfo, CardKey, ctrl);
            }
            else
            {
                RefreshQAEEPROMFAI(baseinfo, CardKey, ctrl);
                RefreshQALabelFAI(baseinfo, CardKey, ctrl);
            }
        }

        private static void RefreshTunableQAFile(ECOBaseInfo baseinfo, string CardKey, Controller ctrl, string srcrootfolder, string eepromfilter)
        {
            if (DirectoryExists(ctrl, srcrootfolder))
            {
                var currentcard = NebulaVM.RetrieveCard(CardKey);
                if (currentcard.Count == 0)
                    return;

                var allattach = NebulaVM.RetrieveCardExistedAttachment(currentcard[0].CardKey);

                var destfolderlist = new List<string>();
                var srcfolders = DirectoryEnumerateDirs(ctrl, srcrootfolder);
                foreach (var fd in srcfolders)
                {
                    var baseinfopn = "";
                    var excelpn = "";

                    var dirs = fd.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    var expn = dirs[dirs.Length - 1].Trim();

                    if (baseinfo.PNDesc.Contains("xx"))
                    {
                        var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                        baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                        if (expn.Length > (lastidx + 2))
                        {
                            excelpn = expn.Remove(lastidx, 2);
                        }
                        else
                        {
                            excelpn = expn;
                        }
                    }
                    else
                    {
                        baseinfopn = baseinfo.PNDesc;
                        excelpn = expn;
                    }

                    if (excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                    {
                        destfolderlist.Add(fd);
                    }
                }//end foreach get folder contains PNDESC

                var destfiles = new List<string>();
                foreach (var fd in destfolderlist)
                {
                    var qafiles = DirectoryEnumerateFiles(ctrl, fd);
                    foreach (var qaf in qafiles)
                    {
                        if (Path.GetFileName(qaf).ToUpper().Contains(eepromfilter.ToUpper()))
                        {
                            destfiles.Add(qaf);
                        }
                    }
                }

                foreach (var desf in destfiles)
                {
                    var fn = Path.GetFileName(desf);
                    fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var pathstrs = desf.Split(Path.DirectorySeparatorChar);
                    var uplevel = pathstrs[pathstrs.Length - 2];
                    var prefix = RMSpectialCh(uplevel);

                    var attfn = prefix + "_" + fn;

                    string datestring = DateTime.Now.ToString("yyyyMMdd");
                    string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                    if (!DirectoryExists(ctrl, imgdir))
                        Directory.CreateDirectory(imgdir);

                    var attpath = imgdir + attfn;
                    var url = "/userfiles/docs/" + datestring + "/" + attfn;

                    var attexist = false;
                    foreach (var att in allattach)
                    {
                        if (att.Contains(attfn))
                        {
                            attexist = true;
                            break;
                        }
                    }

                    if (!attexist)
                    {
                        try
                        {
                            FileCopy(ctrl, desf, attpath, true);
                            NebulaVM.StoreCardAttachment(CardKey, url);

                            var spcard = NebulaVM.RetrieveCard(CardKey);
                            if (spcard.Count > 0
                                && string.Compare(spcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.pending);
                            }
                        }
                        catch (Exception ex) { }
                    }

                }//end foreach

            }//end if

        }

        public static void RefreshTnuableQAFAI(Controller ctrl,ECOBaseInfo baseinfo, string CardKey)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var srcrootfolder = syscfgdict["TUNABLEQAEEPROMFAI"];
            var eepromfilter = syscfgdict["TUNABLEQAEEPROMFILTER"];
            RefreshTunableQAFile(baseinfo, CardKey, ctrl, srcrootfolder, eepromfilter);

            srcrootfolder = syscfgdict["TUNABLEQALABELFAI"];
            eepromfilter = syscfgdict["TUNABLEQALABELFILTER"];
            RefreshTunableQAFile(baseinfo, CardKey, ctrl, srcrootfolder, eepromfilter);
        }

        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static void UpdateOrderInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var currentcard = NebulaVM.RetrieveCard(cardkey);

            var lineiddict = new Dictionary<string, bool>();
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            var desfolder = syscfgdict["ORDERINFOPATH"];
            
            try
            {
                var backlogfiles = new List<string>();
                if (!DirectoryExists(ctrl,imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (DirectoryExists(ctrl,desfolder))
                {
                    var fds = DirectoryEnumerateFiles(ctrl,desfolder);
                    foreach (var fd in fds)
                    {
                        try
                        {
                            var ogfn = Path.GetFileName(fd);
                            if (ogfn.Contains("_0630"))
                            {
                                var fn = imgdir + Path.GetFileName(fd);
                                FileCopy(ctrl,fd, fn, true);
                                backlogfiles.Add(fn);
                            }
                        }
                        catch (Exception ex) { }
                    }//end foreach

                    var alldata = new List<List<string>>();
                    foreach (var bcklog in backlogfiles)
                    {
                        var data = RetrieveDataFromExcel(ctrl,bcklog, null);
                        alldata.AddRange(data);
                    }

                    var ordinfos = new List<MiniPIPOrdeInfo>();
                    foreach (var line in alldata)
                    {
                        var baseinfopn = string.Empty;
                        var excelpn = string.Empty;

                        var expns = line[19].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(var expn in expns)
                        {
                            if (baseinfo.PNDesc.Contains("xx"))
                            {
                                var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                                baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                                if (expn.Length > (lastidx + 2))
                                {
                                   excelpn = expn.Remove(lastidx, 2);
                                }
                                else
                                {
                                    excelpn = expn;
                                }
                            }
                            else
                            {
                                 baseinfopn= baseinfo.PNDesc;
                                 excelpn = expn;
                            }


                            if (excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                            {
                                if (lineiddict.ContainsKey(line[38]))
                                {
                                    break;
                                }
                                else
                                {
                                    lineiddict.Add(line[38], true);
                                }

                                var tempinfo = new MiniPIPOrdeInfo();
                                tempinfo.CardKey = cardkey;
                                tempinfo.OrderType = line[0];
                                tempinfo.OrderDate = line[1];
                                tempinfo.SONum = line[15];
                                tempinfo.Item = line[17];
                                tempinfo.Description = line[19];
                                tempinfo.QTY = line[21];
                                tempinfo.SSD = line[25];
                                tempinfo.Planner = line[27];
                                tempinfo.LineID = line[38];

                                ordinfos.Add(tempinfo);
                                break;
                            }//end if
                        }//end foreach
                    }//end foreach

                    if (ordinfos.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                        }
                    }

                    NebulaVM.UpdateOrderInfo(ordinfos, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static void UpdateJOInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var currentcard = NebulaVM.RetrieveCard(cardkey);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            var desfile = syscfgdict["JOORDERINFO"];
            var jolist = new List<ECOJOOrderInfo>();           

            try
            {
                var backlogfiles = new List<string>();
                if (!DirectoryExists(ctrl,imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (FileExist(ctrl,desfile))
                {
                    try
                    {
                        var fn = imgdir + Path.GetFileName(desfile);
                        FileCopy(ctrl,desfile, fn, true);
                        var data = RetrieveDataFromExcel(ctrl,fn, null);
                        foreach (var line in data)
                        {
                            var baseinfopn = string.Empty;
                            var excelpn = string.Empty;

                            var expns = line[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var expn in expns)
                            {
                                if (baseinfo.PNDesc.Contains("xx"))
                                {
                                    var lastidx = baseinfo.PNDesc.LastIndexOf("xx");
                                    baseinfopn = baseinfo.PNDesc.Remove(lastidx, 2);

                                    if (expn.Length > (lastidx + 2))
                                    {
                                        excelpn = expn.Remove(lastidx, 2);
                                    }
                                    else
                                    {
                                        excelpn = expn;
                                    }
                                }
                                else
                                {
                                    baseinfopn = baseinfo.PNDesc;
                                    excelpn = expn;
                                }

                                if ((string.Compare(line[3], "903") == 0 || string.Compare(line[3], "919") == 0)
                                && excelpn.ToUpper().Contains(baseinfopn.ToUpper()))
                                {
                                    var tempinfo = new ECOJOOrderInfo();
                                    tempinfo.Description = line[1];
                                    tempinfo.Category = line[3];
                                    tempinfo.WipJob = line[11];
                                    tempinfo.JobStatus = line[14];
                                    tempinfo.SSTD = line[15];
                                    tempinfo.RleaseDate = line[17];
                                    tempinfo.JOQTY = line[20];
                                    tempinfo.Planner = line[37];
                                    tempinfo.Creator = line[38];
                                    jolist.Add(tempinfo);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }


                    if (jolist.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                        }
                    }

                    NebulaVM.UpdateJOInfo(jolist, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static void Update1STJOCheckFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = NebulaVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, true);
                    }
                }

                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var currentcard = NebulaVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfile = syscfgdict["JO1STCHECKINFO"];

                var jochecklist = new List<ECOJOCheck>();

                try
                {
                    if (!DirectoryExists(ctrl,imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    if (FileExist(ctrl,desfile))
                    {
                        try
                        {
                            var fn = imgdir + Path.GetFileName(desfile);
                            FileCopy(ctrl,desfile, fn, true);

                            var data = RetrieveDataFromExcel(ctrl,fn, null);
                            foreach (var line in data)
                            {
                                if (jodict.ContainsKey(line[1]))
                                {
                                    var tempinfo = new ECOJOCheck();
                                    tempinfo.CardKey = cardkey;
                                    tempinfo.CheckType = NebulaJOCHECKTYPE.ENGTYPE;
                                    tempinfo.JO = line[1];
                                    tempinfo.EEPROM = line[4];
                                    tempinfo.EEPROMDT = line[6];
                                    tempinfo.Label = line[7];
                                    tempinfo.LabelDT = line[8];
                                    tempinfo.TestData = line[9];
                                    tempinfo.TestDateDT = line[10];
                                    tempinfo.Costemic = line[11];
                                    tempinfo.CostemicDT = line[12];
                                    tempinfo.Status = line[13];
                                    jochecklist.Add(tempinfo);
                                }
                            }
                        }
                        catch (Exception ex) { }

                        if (jochecklist.Count > 0)
                        {
                            NebulaVM.UpdateJOCheckInfo(jochecklist, cardkey);
                        }

                        if (jochecklist.Count > 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                            }
                        }

                        //if (jochecklist.Count == 0 && currentcard.Count > 0)
                        //{
                        //    if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.pending) == 0)
                        //    {
                        //        NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.working);
                        //    }
                        //}
                    }
                }
                catch (Exception ex) { }
            }
        }

        public static void Update2NDJOCheckFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = NebulaVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, true);
                    }
                }

                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var currentcard = NebulaVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfolder = syscfgdict["JO2NDCHECKINFO"];

                var jochecklist = new List<ECOJOCheck>();

                try
                {
                    if (!DirectoryExists(ctrl,imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    var fs = DirectoryEnumerateFiles(ctrl,desfolder);
                    var desfile = string.Empty;
                    foreach (var f in fs)
                    {
                        if (Path.GetFileName(f).Contains("FAI OQC"))
                        {
                            desfile = f;
                            break;
                        }
                    }

                    if (FileExist(ctrl,desfile))
                    {
                        try
                        {
                            var fn = imgdir + "QA_FAI_OCQ"+Path.GetExtension(desfile);
                            FileCopy(ctrl,desfile, fn, true);

                            var data = RetrieveDataFromExcel(ctrl,fn, null);
                            foreach (var line in data)
                            {
                                var jos = line[9].Split(new string[] { "\r", "\n"," ","," }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var j in jos)
                                {
                                    if (jodict.ContainsKey(j.Trim()))
                                    {
                                        var tempinfo = new ECOJOCheck();
                                        tempinfo.CardKey = cardkey;
                                        tempinfo.CheckType = NebulaJOCHECKTYPE.QATYPE;
                                        tempinfo.JO = j.Trim();
                                        tempinfo.EEPROM = line[5];
                                        tempinfo.EEPROMDT = line[0] + "-" + line[1] + "-" + line[2];
                                        jochecklist.Add(tempinfo);
                                    }
                                }

                            }
                        }
                        catch (Exception ex) { }

                        if (jochecklist.Count > 0)
                        {
                            NebulaVM.UpdateJOCheckInfo(jochecklist, cardkey);
                        }

                        if (jochecklist.Count > 0 && currentcard.Count > 0)
                        {
                            if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                            }
                        }

                        //if (jochecklist.Count == 0 && currentcard.Count > 0)
                        //{
                        //    if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.pending) == 0)
                        //    {
                        //        NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.working);
                        //    }
                        //}
                    }
                }
                catch (Exception ex) { }
            }

        }

        public static void UpdateJOMainStoreFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var JoTable = NebulaVM.RetrieveJOInfo(cardkey);
            if (JoTable.Count > 0)
            {
                var jodict = new Dictionary<string, bool>();
                foreach (var jo in JoTable)
                {
                    if (!jodict.ContainsKey(jo.WipJob))
                    {
                        jodict.Add(jo.WipJob, false);
                    }
                }

                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var currentcard = NebulaVM.RetrieveCard(cardkey);

                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var desfile = syscfgdict["JO2MAINSTORE"];

                try
                {
                    if (!DirectoryExists(ctrl,imgdir))
                    {
                        Directory.CreateDirectory(imgdir);
                    }

                    if (FileExist(ctrl,desfile))
                    {
                        try
                        {
                            var fn = imgdir + Path.GetFileName(desfile);
                            FileCopy(ctrl,desfile, fn, true);

                            var data = RetrieveDataFromExcel(ctrl,fn, null);
                            foreach (var line in data)
                            {
                                if (jodict.ContainsKey(line[11]))
                                {
                                    if (!line[14].ToUpper().Contains("COMPLETE"))
                                    {
                                        jodict[line[11]] = true;
                                    }
                                }
                            }


                            foreach (var kv in jodict)
                            {
                                if (kv.Value)
                                {
                                    NebulaVM.UpdateJOMainStore(cardkey, kv.Key, NebulaJOSTORESTATUS.WIP);
                                }
                                else
                                {
                                    NebulaVM.UpdateJOMainStore(cardkey, kv.Key, NebulaJOSTORESTATUS.STORED);
                                }
                            }//end foreach
                        }
                        catch (Exception ex) { }
                    }
                }
                catch (Exception ex) { }
            }
        }

        public static void UpdateShipInfoFromExcel(Controller ctrl, ECOBaseInfo baseinfo, string cardkey)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var ordercard = NebulaVM.RetrieveSpecialCard(baseinfo, NebulaCardType.SampleOrdering);
            var orderinfos = NebulaVM.RetrieveOrderInfo(ordercard[0].CardKey);
            var sodict = new Dictionary<string, bool>();
            foreach (var so in orderinfos)
            {
                if (!sodict.ContainsKey(so.SONum))
                {
                    sodict.Add(so.SONum, true);
                }
            }

            var currentcard = NebulaVM.RetrieveCard(cardkey);

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            //var desfile = syscfgdict["SHIPMENTINFOPATH"];
            //var sheetname = syscfgdict["SHIPMENTINFOSHEETNAME"];

            var desfolder = syscfgdict["SHIPMENTINFOPATH"];

            var jolist = new List<ECOShipInfo>();

            try
            {
                if (!DirectoryExists(ctrl,imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                var fs = DirectoryEnumerateFiles(ctrl,desfolder);
                var desfile = string.Empty;
                foreach (var f in fs)
                {
                    if (Path.GetFileName(f).Contains("Finisar - OTS"))
                    {
                        desfile = f;
                        break;
                    }
                }
                

                if (FileExist(ctrl,desfile))
                {
                    var sheetname = Path.GetFileNameWithoutExtension(desfile);
                    try
                    {
                        var fn = imgdir + Path.GetFileName(desfile);
                        FileCopy(ctrl,desfile, fn, true);

                        var data = RetrieveDataFromExcel(ctrl,fn, sheetname);
                        foreach (var line in data)
                        {
                            if (sodict.ContainsKey(line[8]))
                            {
                                var tempinfo = new ECOShipInfo();
                                tempinfo.ShipSONum = line[8];
                                tempinfo.ShipLine = line[9];
                                tempinfo.ShipOrderQTY = line[13];
                                tempinfo.ShippedQTY = line[14];
                                tempinfo.ShipDate = line[19];
                                jolist.Add(tempinfo);
                            }
                        }
                    }
                    catch (Exception ex) { }


                    if (jolist.Count > 0 && currentcard.Count > 0)
                    {
                        if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(cardkey, NebulaCardStatus.pending);
                        }
                    }

                    NebulaVM.UpdateShipInfo(jolist, cardkey);
                }
            }
            catch (Exception ex) { }

        }

        public static List<QACheckData> RetrieveAllQACheckInfo(Controller ctrl)
        {
            var ret = new List<QACheckData>();
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["QAFACHECKCHART"];
            var sheetname = syscfgdict["QAFACHECKCHARTSHEET"];
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            try
            {
                if (!DirectoryExists(ctrl,imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }

                if (FileExist(ctrl,srcfile))
                {
                    try
                    {
                        var fn = imgdir + Path.GetFileNameWithoutExtension(srcfile) + DateTime.Now.ToString("yyyyMMddhhmmss")+ Path.GetExtension(srcfile);
                        FileCopy(ctrl,srcfile, fn, true);
                        var data = RetrieveDataFromExcel(ctrl,fn, sheetname);
                        foreach (var line in data)
                        {
                            try
                            {
                                var tempinfo = new QACheckData();
                                tempinfo.QADate = DateTime.Parse(line[0] + "-" + line[1] + "-" + line[2] + " 07:30:00");
                                if (line[5].Contains("@"))
                                {
                                    tempinfo.PE = line[5].ToUpper();
                                }
                                else
                                {
                                    tempinfo.PE = (line[5].Replace(" ", ".") + "@finisar.com").ToUpper();
                                }

                                if (line[4].ToUpper().Contains("EEPROM") && line[4].ToUpper().Contains("FLI"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.EEPROMPASS = Convert.ToInt32(line[11]);
                                        tempinfo.FLIPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.EEPROMFAIL = Convert.ToInt32(line[12]);
                                        tempinfo.FLIFAIL = Convert.ToInt32(line[12]);
                                    }

                                }
                                else if (line[4].ToUpper().Contains("EEPROM"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.EEPROMPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.EEPROMFAIL = Convert.ToInt32(line[12]);
                                    }
                                }
                                else if (line[4].ToUpper().Contains("FLI"))
                                {
                                    if (!string.IsNullOrEmpty(line[11]))
                                    {
                                        tempinfo.FLIPASS = Convert.ToInt32(line[11]);
                                    }
                                    if (!string.IsNullOrEmpty(line[12]))
                                    {
                                        tempinfo.FLIFAIL = Convert.ToInt32(line[12]);
                                    }
                                }
                                ret.Add(tempinfo);
                            }
                            catch (Exception ex) { }
                        }//end foreach
                    }
                    catch (Exception ex) { }

                }
            }
            catch (Exception ex) { }

            return ret;
        }

    }
}