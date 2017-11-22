using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Globalization;

namespace Nebula.Models
{
    public class BRJOSYSTEMSTATUS
    {
        public static string OPEN = "OPEN";
        public static string CLOSE = "CLOSE";
        public static string KICKOFF = "KICKOFF";
    }

    public class AGILEBRSTATUS
    {
        public static string APPROVE2BUILE = "Approved To Build";
    }

    public class BRAgileVM
    {
        private static List<BRAgileBaseInfo> ParseBR(string BRLIST,Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();
            var brs = BRLIST.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
            return ParseBR(brs.ToList(), ctrl);
        }

        private static List<BRAgileBaseInfo> ParseBR(List<string> brs, Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();
            BRAgileBaseInfo tempinfo = null;
            foreach (var br in brs)
            {
                tempinfo = null;
                string brfile = ctrl.Server.MapPath("~/userfiles") + "\\docs\\Agile\\" + br.Trim() + "\\" + br.Trim() + "-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (File.Exists(brfile))
                {
                    var alllines = File.ReadAllLines(brfile);
                    foreach (var line in alllines)
                    {
                        if (line.Contains("<<BASEINFO>>"))
                        {
                            tempinfo = BRAgileBaseInfo.ParseItem(line.Replace("<<BASEINFO>>", "").Trim());
                        }

                        if (line.Contains("<<WORKFLOW>>") && tempinfo != null)
                        {
                            var wf = AgileWorkFlow.ParseItem(line.Replace("<<WORKFLOW>>", "").Trim());
                            tempinfo.brworkflowlist.Add(wf);
                        }

                        if (line.Contains("<<AFFECT>>") && tempinfo != null)
                        {
                            var aa = AgileAffectItem.ParseItem(line.Replace("<<AFFECT>>", "").Trim());
                            tempinfo.affectitem.Add(aa);
                        }

                        if (line.Contains("<<HISTORY>>") && tempinfo != null)
                        {
                            var ah = AgileHistory.ParseItem(line.Replace("<<HISTORY>>", "").Trim());
                            tempinfo.history.Add(ah);
                        }

                        if (line.Contains("<<ATTACH>>") && tempinfo != null)
                        {
                            var at = AgileAttach.ParseItem(line.Replace("<<ATTACH>>", "").Trim());
                            tempinfo.attach.Add(at);
                        }
                    }//end foreach

                    if (tempinfo != null)
                        ret.Add(tempinfo);
                }
            }
            return ret;
        }

        public static void LoadNewBR(string BRLIST, Controller ctrl)
        {
            var newbrlist =  ParseBR(BRLIST, ctrl);
            foreach (var br in newbrlist)
            {
                AgileDownloadVM.UpdatePMLastUpdateTime(br.Originator, br.OriginalDate);
                if (!string.IsNullOrEmpty(br.Description))
                {
                    if (BRAgileBaseInfo.BRExist(br.BRNumber,br.OriginalDate))
                    {
                        br.UpdateBRAgileInfo();
                    }
                    else
                    {
                        br.AddBRAgileInfo();
                    }
                }//end if
            }//end foreach
        }

        public static void UpdateBR(Controller ctrl)
        {
            var brlist = BRAgileBaseInfo.RetrieveBRNumNeedToUpdate();
            var newbrlist = ParseBR(brlist, ctrl);
            foreach (var br in newbrlist)
            {
                br.UpdateBRAgileInfo();
            }
        }

        public static DateTime ConvertUSLocalToDate(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Trim()))
                {
                    return DateTime.Parse("1982-05-06 10:00:00");
                }
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                var date = DateTime.ParseExact(obj.Trim().Replace("CST", "-6"), "ddd MMM dd HH:mm:ss z yyyy", culture);
                return date;
            }
            catch (Exception ex)
            {
                return DateTime.Parse("1982-05-06 10:00:00");
            }
        }
    }


    public class AgileWorkFlow
    {
        public AgileWorkFlow()
        {
            StatusCode = "";
            WorkFlow = "";
            WorkFlowStatus = "";
            Action = "";
            Reqd = "";
            Reviewer = "";
            SignoffUser = "";
            StatusChangedBy = "";
            LocalTime = DateTime.Parse("1982-05-06 10:00:00");
            SignoffComment = "";
            SignoffDuration = "";
        }
        public static AgileWorkFlow ParseItem(string line)
        {
            var item = new AgileWorkFlow();
            var splitstrs = line.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in splitstrs)
            {
                var kv = sp.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                var value = string.Empty;
                if (kv.Length > 1) value = kv[1].Replace("'", "");

                if (string.Compare(kv[0], "StatusCode", true) == 0)
                    item.StatusCode = value;
                if (string.Compare(kv[0], "WorkFlow", true) == 0)
                    item.WorkFlow = value;
                if (string.Compare(kv[0], "WorkFlowStatus", true) == 0)
                    item.WorkFlowStatus = value;
                if (string.Compare(kv[0], "Action", true) == 0)
                    item.Action = value;
                if (string.Compare(kv[0], "Reqd", true) == 0)
                    item.Reqd = value;
                if (string.Compare(kv[0], "Reviewer", true) == 0)
                    item.Reviewer = value;
                if (string.Compare(kv[0], "SignoffUser", true) == 0)
                    item.SignoffUser = value;
                if (string.Compare(kv[0], "StatusChangedBy", true) == 0)
                    item.StatusChangedBy = value;
                if (string.Compare(kv[0], "LocalTime", true) == 0)
                    item.LocalTime = BRAgileVM.ConvertUSLocalToDate(value);
                if (string.Compare(kv[0], "SignoffComment", true) == 0)
                    item.SignoffComment = value;
                if (string.Compare(kv[0], "SignoffDuration", true) == 0)
                    item.SignoffDuration = value;
            }
            return item;
        }

        public void AddBRAgileInfo(string brkey,string BRNumber)
        {
            var sql = "insert into AgileWorkFlow(BRKey,BRNumber,StatusCode,WorkFlow,WorkFlowStatus,Action,Reqd,Reviewer,SignoffUser,StatusChangedBy,SignoffComment,SignoffDuration,LocalTime) "
                + " values('<BRKey>','<BRNumber>','<StatusCode>','<WorkFlow>','<WorkFlowStatus>','<Action>','<Reqd>','<Reviewer>','<SignoffUser>','<StatusChangedBy>',N'<SignoffComment>','<SignoffDuration>','<LocalTime>')";
            sql = sql.Replace("<BRKey>", brkey).Replace("<BRNumber>", BRNumber).Replace("<StatusCode>", StatusCode).Replace("<WorkFlow>", WorkFlow)
                .Replace("<WorkFlowStatus>", WorkFlowStatus).Replace("<Action>", Action).Replace("<Reqd>", Reqd).Replace("<Reviewer>", Reviewer)
                .Replace("<SignoffUser>", SignoffUser).Replace("<StatusChangedBy>", StatusChangedBy).Replace("<SignoffComment>", SignoffComment).Replace("<SignoffDuration>", SignoffDuration)
                .Replace("<LocalTime>", LocalTime.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public static void RemoveBRAgileInfo(string brkey)
        {
            var sql = "delete from AgileWorkFlow where BRKey = '<BRKey>'";
            sql = sql.Replace("<BRKey>", brkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public string StatusCode{set;get;}
        public string WorkFlow{set;get;}
        public string WorkFlowStatus{set;get;}
        public string Action{set;get;}
        public string Reqd{set;get;}
        public string Reviewer{set;get;}
        public string SignoffUser{set;get;}
        public string StatusChangedBy{set;get;}
        public DateTime LocalTime{set;get;}
        public string SignoffComment{set;get;}
        public string SignoffDuration{set;get;}
    }
    public class AgileAffectItem
    {
        public AgileAffectItem()
        {
            PN = "";
            itemsite = "";
            itemdesc = "";
            lifecycle = "";
            commodity = "";
        }

        public static AgileAffectItem ParseItem(string line)
        {
            var item = new AgileAffectItem();
            var splitstrs = line.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in splitstrs)
            {
                var kv = sp.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                var value = string.Empty;
                if (kv.Length > 1) value = kv[1].Replace("'", "");

                if (string.Compare(kv[0], "itemnumber", true) == 0)
                    item.PN = value;
                if (string.Compare(kv[0], "itemsite", true) == 0)
                    item.itemsite = value;
                if (string.Compare(kv[0], "itemdesc", true) == 0)
                    item.itemdesc = value;
                if (string.Compare(kv[0], "lifecycle", true) == 0)
                    item.lifecycle = value;
                if (string.Compare(kv[0], "commodity", true) == 0)
                    item.commodity = value;
            }
            return item;
        }

        public void AddBRAgileInfo(string brkey, string BRNumber)
        {
            var sql = "insert into AgileAffectItem(BRKey,BRNumber,PN,itemsite,itemdesc,lifecycle,commodity) "
                + " values('<BRKey>','<BRNumber>','<PN>','<itemsite>',N'<itemdesc>','<lifecycle>','<commodity>')";
            sql = sql.Replace("<BRKey>", brkey).Replace("<BRNumber>", BRNumber).Replace("<PN>", PN).Replace("<itemsite>", itemsite)
                .Replace("<itemdesc>", itemdesc).Replace("<lifecycle>", lifecycle).Replace("<commodity>", commodity);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public static void RemoveBRAgileInfo(string brkey)
        {
            var sql = "delete from AgileAffectItem where BRKey = '<BRKey>'";
            sql = sql.Replace("<BRKey>", brkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public string PN{set;get;}
        public string itemsite{set;get;}
        public string itemdesc{set;get;}
        public string lifecycle{set;get;}
        public string commodity{set;get;}
    }

    public class AgileHistory
    {
        public AgileHistory()
        {
            status = "";
            nextstatus = "";
            action = "";
            actionuser = "";
            localtime = DateTime.Parse("1982-05-06 10:00:00"); ;
            detail = "";
            usernotice = "";
        }

        public static AgileHistory ParseItem(string line)
        {
            var item = new AgileHistory();
            var splitstrs = line.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in splitstrs)
            {
                var kv = sp.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                var value = string.Empty;
                if (kv.Length > 1) value = kv[1].Replace("'","");

                if (string.Compare(kv[0], "status", true) == 0)
                    item.status = value;
                if (string.Compare(kv[0], "nextstatus", true) == 0)
                    item.nextstatus = value;
                if (string.Compare(kv[0], "action", true) == 0)
                    item.action = value;
                if (string.Compare(kv[0], "user", true) == 0)
                    item.actionuser = value;
                if (string.Compare(kv[0], "localtime", true) == 0)
                    item.localtime = BRAgileVM.ConvertUSLocalToDate(value);
                if (string.Compare(kv[0], "detail", true) == 0)
                    item.detail = value;
                if (string.Compare(kv[0], "usernotice", true) == 0)
                    item.usernotice = value;
            }
            return item;
        }

        public void AddBRAgileInfo(string brkey, string BRNumber)
        {
            var sql = "insert into AgileHistory(BRKey,BRNumber,status,nextstatus,action,actionuser,localtime,detail,usernotice) "
                + " values('<BRKey>','<BRNumber>','<status>','<nextstatus>','<action>','<actionuser>','<localtime>',N'<detail>','<usernotice>')";
            sql = sql.Replace("<BRKey>", brkey).Replace("<BRNumber>", BRNumber).Replace("<status>", status).Replace("<nextstatus>", nextstatus)
                .Replace("<action>", action).Replace("<actionuser>", actionuser).Replace("<localtime>", localtime.ToString()).Replace("<detail>", detail)
                .Replace("<usernotice>", usernotice);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public static void RemoveBRAgileInfo(string brkey)
        {
            var sql = "delete from AgileHistory where BRKey = '<BRKey>'";
            sql = sql.Replace("<BRKey>", brkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public string status{set;get;}
        public string nextstatus{set;get;}
        public string action{set;get;}
        public string actionuser{set;get;}
        public DateTime localtime{set;get;}
        public string detail{set;get;}
        public string usernotice{set;get;}
    }

    public class AgileAttach
    {
        public AgileAttach()
        {
            FileName = "";
            ModifyDate = DateTime.Parse("1982-05-06 10:00:00"); 
            Checkiner = "";
            LocalFilePath = "";
        }
        public static AgileAttach ParseItem(string line)
        {
            var item = new AgileAttach();
            var splitstrs = line.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in splitstrs)
            {
                var kv = sp.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                var value = string.Empty;
                if (kv.Length > 1) value = kv[1].Replace("'", "");

                if (string.Compare(kv[0], "FileName", true) == 0)
                    item.FileName = value;
                if (string.Compare(kv[0], "ModifyDate", true) == 0)
                    item.ModifyDate = BRAgileVM.ConvertUSLocalToDate(value);
                if (string.Compare(kv[0], "Checkiner", true) == 0)
                    item.Checkiner = value;
                if (string.Compare(kv[0], "LocalFilePath", true) == 0)
                    item.LocalFilePath = value;
            }
            return item;
        }

        public void AddBRAgileInfo(string brkey, string BRNumber)
        {
            var sql = "insert into AgileAttach(BRKey,BRNumber,FileName,LocalFilePath,Checkiner,ModifyDate) "
                + " values('<BRKey>','<BRNumber>','<FileName>','<LocalFilePath>','<Checkiner>','<ModifyDate>')";
            sql = sql.Replace("<BRKey>", brkey).Replace("<BRNumber>", BRNumber).Replace("<FileName>", FileName).Replace("<LocalFilePath>", LocalFilePath)
                .Replace("<Checkiner>", Checkiner).Replace("<ModifyDate>", ModifyDate.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public static void RemoveBRAgileInfo(string brkey)
        {
            var sql = "delete from AgileAttach where BRKey = '<BRKey>'";
            sql = sql.Replace("<BRKey>", brkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }
        public string FileName{set;get;}
        public DateTime ModifyDate{set;get;}
        public string Checkiner{set;get;}
        public string LocalFilePath{set;get;}
    }

    public class BRAgileBaseInfo
    {
        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public BRAgileBaseInfo()
        {
            ChangeType = "";
            BRNumber = "";
            Description = "";
            Status = "";
            Workflow = "";
            Originator = "";
            OriginalDate = DateTime.Parse("1982-05-06 10:00:00");
            BRKey = "";
            brworkflowlist = new List<AgileWorkFlow>();
            affectitem = new List<AgileAffectItem>();
            history = new List<AgileHistory>();
            attach = new List<AgileAttach>();
            BRStatus = "";
            ProjectKey = "";
        }

        public static BRAgileBaseInfo ParseItem(string line)
        {
            var item = new BRAgileBaseInfo();
            var splitstrs = line.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sp in splitstrs)
            {
                var kv = sp.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                var value = string.Empty;
                if (kv.Length > 1) value = kv[1].Replace("'", "");

                if (string.Compare(kv[0], "ChangeType", true) == 0)
                    item.ChangeType = value;
                if (string.Compare(kv[0], "Number", true) == 0)
                    item.BRNumber = value;
                if (string.Compare(kv[0], "Description", true) == 0)
                    item.Description = value;
                if (string.Compare(kv[0], "Status", true) == 0)
                    item.Status = value;
                if (string.Compare(kv[0], "Workflow", true) == 0)
                    item.Workflow = value;
                if (string.Compare(kv[0], "Originator", true) == 0)
                    item.Originator = value;
                if (string.Compare(kv[0], "OriginalDate", true) == 0)
                    item.OriginalDate = BRAgileVM.ConvertUSLocalToDate(value);
            }
            return item;
        }

        public static bool BRExist(string brnum, DateTime originaltime)
        {
            var sql = "select BRNumber from BRAgileBaseInfo where BRNumber = '<BRNumber>' and OriginalDate = '<OriginalDate>'";
            sql = sql.Replace("<BRNumber>",brnum).Replace("<OriginalDate>", originaltime.ToString("yyyy-MM-dd HH:mm:ss"));
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
                return true;
            else
                return false;
        }

        public void AddBRAgileInfo()
        {
            var brkey = GetUniqKey();
            var sql = "insert into BRAgileBaseInfo(BRKey,BRNumber,Description,Status,Workflow,Originator,ChangeType,OriginalDate,BRStatus) "
                + " values('<BRKey>','<BRNumber>',N'<Description>','<Status>','<Workflow>','<Originator>','<ChangeType>','<OriginalDate>','<BRStatus>')";
            sql = sql.Replace("<BRKey>",brkey).Replace("<BRNumber>", BRNumber).Replace("<Description>", Description)
                .Replace("<Status>", Status).Replace("<Workflow>", Workflow).Replace("<Originator>", Originator)
                .Replace("<ChangeType>", ChangeType).Replace("<OriginalDate>", OriginalDate.ToString("yyyy-MM-dd HH:mm:ss")).Replace("<BRStatus>",BRJOSYSTEMSTATUS.KICKOFF);
            DBUtility.ExeLocalSqlNoRes(sql);

            foreach (var item in brworkflowlist)
            { item.AddBRAgileInfo(brkey, BRNumber); }
            foreach (var item in affectitem)
            { item.AddBRAgileInfo(brkey, BRNumber); }
            foreach (var item in history)
            { item.AddBRAgileInfo(brkey, BRNumber); }
            foreach (var item in attach)
            { item.AddBRAgileInfo(brkey, BRNumber); }
        }

        public static void UpdateBRStatus(string brkey, string status)
        {
            var sql = "update BRAgileBaseInfo set BRStatus = '<BRStatus>' where BRKey = '<BRKey>'";
            sql = sql.Replace("<BRKey>", brkey).Replace("<BRStatus>", status);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public void UpdateBRAgileInfo()
        {
            if (brworkflowlist.Count > 0)
            {
                var currentstatus = brworkflowlist[0].WorkFlowStatus;
                var sql = "update BRAgileBaseInfo set Status = '<Status>' where BRNumber = '<BRNumber>'";
                sql = sql.Replace("<BRNumber>", BRNumber).Replace("<Status>", currentstatus);
                DBUtility.ExeLocalSqlNoRes(sql);

                sql = "select BRKey from BRAgileBaseInfo where BRNumber = '<BRNumber>'";
                sql = sql.Replace("<BRNumber>", BRNumber);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                if (dbret.Count > 0)
                {
                    var brkey = Convert.ToString(dbret[0][0]);
                    AgileWorkFlow.RemoveBRAgileInfo(brkey);
                    AgileAffectItem.RemoveBRAgileInfo(brkey);
                    AgileHistory.RemoveBRAgileInfo(brkey);
                    AgileAttach.RemoveBRAgileInfo(brkey);

                    foreach (var item in brworkflowlist)
                    { item.AddBRAgileInfo(brkey, BRNumber); }
                    foreach (var item in affectitem)
                    { item.AddBRAgileInfo(brkey, BRNumber); }
                    foreach (var item in history)
                    { item.AddBRAgileInfo(brkey, BRNumber); }
                    foreach (var item in attach)
                    { item.AddBRAgileInfo(brkey, BRNumber); }
                }
            }
        }

        public static List<string> RetrieveBRNumNeedToUpdate()
        {
            var sql = "select BRNumber from BRAgileBaseInfo where Status <> '<Status>' and OriginalDate > '<threemonth>'";
            sql = sql.Replace("<Status>", AGILEBRSTATUS.APPROVE2BUILE).Replace("<threemonth>", DateTime.Now.AddMonths(-3).ToString());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<string>();
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

        public static Dictionary<string, string> RetrieveAllBRDictIn3Month()
        {
            var ret = new Dictionary<string, string>();
            var sql = "select BRKey,BRNumber from BRAgileBaseInfo where OriginalDate > '<threemonth>'";
            sql = sql.Replace("<threemonth>", DateTime.Now.AddMonths(-3).ToString());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var brnum = Convert.ToString(line[1]);
                var brkey = Convert.ToString(line[0]);
                if(!ret.ContainsKey(brnum))
                    ret.Add(brnum, brkey);
            }
            return ret;
        }

        public static List<BRAgileBaseInfo> RetrieveActiveBRAgileInfo(string reviewer, Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();

            var sql = string.Empty;
            if (reviewer == null)
            {
                sql = "select BRKey,BRNumber,Description,Status,Originator,OriginalDate,BRStatus,ProjectKey from BRAgileBaseInfo order by OriginalDate Desc";
            }
            else
            {
                reviewer = reviewer.Replace("@FINISAR.COM", "").Replace(".", " ");
                sql = "select BRKey,BRNumber,Description,Status,Originator,OriginalDate,BRStatus,ProjectKey from BRAgileBaseInfo where Originator = '<Originator>'  order by OriginalDate Desc";
                sql = sql.Replace("<Originator>", reviewer);
            }

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new BRAgileBaseInfo();
                temp.BRKey = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.Description = Convert.ToString(line[2]);
                temp.Status = Convert.ToString(line[3]);
                temp.Originator = Convert.ToString(line[4]);
                temp.OriginalDate = Convert.ToDateTime(line[5]);
                temp.BRStatus = Convert.ToString(line[6]);
                temp.ProjectKey = Convert.ToString(line[7]);
                temp.CommentList = RetriveBRComment(temp.BRNumber,ctrl);
                ret.Add(temp);
            }

            return ret;
        }

        public static List<BRAgileBaseInfo> RetrieveActiveBRAgileInfoWithStatus(string reviewer,string BRStatus, Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();

            var sql = string.Empty;
            if (reviewer == null)
            {
                sql = "select BRKey,BRNumber,Description,Status,Originator,OriginalDate,BRStatus,ProjectKey from BRAgileBaseInfo where BRStatus = '<BRStatus>' order by OriginalDate Desc";
                sql = sql.Replace("<BRStatus>",BRStatus);
            }
            else
            {
                reviewer = reviewer.Replace("@FINISAR.COM", "").Replace(".", " ");
                sql = "select BRKey,BRNumber,Description,Status,Originator,OriginalDate,BRStatus,ProjectKey from BRAgileBaseInfo where Originator = '<Originator>' and BRStatus = '<BRStatus>' order by OriginalDate Desc";
                sql = sql.Replace("<Originator>", reviewer).Replace("<BRStatus>", BRStatus);
            }

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new BRAgileBaseInfo();
                temp.BRKey = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.Description = Convert.ToString(line[2]);
                temp.Status = Convert.ToString(line[3]);
                temp.Originator = Convert.ToString(line[4]);
                temp.OriginalDate = Convert.ToDateTime(line[5]);
                temp.BRStatus = Convert.ToString(line[6]);
                temp.ProjectKey = Convert.ToString(line[7]);
                temp.CommentList = RetriveBRComment(temp.BRNumber,ctrl);
                ret.Add(temp);
            }

            return ret;
        }

        public static List<BRAgileBaseInfo> RetrieveBRAgileInfo(string BRNum, Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();

            var sql = "select BRKey,BRNumber,Description,Status,Originator,OriginalDate,BRStatus,ProjectKey from BRAgileBaseInfo where BRNumber like '%<BRNumber>%'";
            sql = sql.Replace("<BRNumber>", BRNum);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new BRAgileBaseInfo();
                temp.BRKey = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.Description = Convert.ToString(line[2]);
                temp.Status = Convert.ToString(line[3]);
                temp.Originator = Convert.ToString(line[4]);
                temp.OriginalDate = Convert.ToDateTime(line[5]);
                temp.BRStatus = Convert.ToString(line[6]);
                temp.ProjectKey = Convert.ToString(line[7]);
                temp.CommentList = RetriveBRComment(temp.BRNumber,ctrl);
                ret.Add(temp);
            }

            return ret;
        }

        public static void UpdateBRProjectkey(string BRNum,string pjkey)
        {
            var sql = "update BRAgileBaseInfo set ProjectKey = '<ProjectKey>' where BRNumber = '<BRNumber>'";
            sql = sql.Replace("<BRNumber>", BRNum).Replace("<ProjectKey>", pjkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public List<BRComment> GeneralCommentList
        { get { return generalcommentlist; } }

        public List<BRComment> ConclusionCommentList
        { get { return conclusioncommentlist; } }

        private List<BRComment> generalcommentlist = new List<BRComment>();
        private List<BRComment> conclusioncommentlist = new List<BRComment>();

        private List<BRComment> cemlist = new List<BRComment>();
        public List<BRComment> CommentList
        {
            set
            {
                cemlist.Clear();
                cemlist.AddRange(value);
                generalcommentlist.Clear();
                conclusioncommentlist.Clear();

                foreach (var item in cemlist)
                {
                    if (string.Compare(item.CommentType, BRCOMMENTTP.COMMENT) == 0
                        || string.IsNullOrEmpty(item.CommentType))
                    {
                        generalcommentlist.Add(item);
                    }
                    if (string.Compare(item.CommentType, BRCOMMENTTP.CONCLUSION) == 0)
                    {
                        conclusioncommentlist.Add(item);
                    }
                }
            }

            get
            {
                return cemlist;
            }
        }

        public static void StoreBRComment(string BRNum, string dbComment, string CommentType, string Reporter)
        {
            var sql = "insert into BRComment(BRNum,CommentKey,Comment,Reporter,CommentDate,CommentType) values('<BRNum>','<CommentKey>','<Comment>','<Reporter>','<CommentDate>','<CommentType>')";
            sql = sql.Replace("<BRNum>", BRNum).Replace("<CommentKey>", GetUniqKey()).Replace("<Comment>", dbComment)
                .Replace("<Reporter>", Reporter).Replace("<CommentDate>", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Replace("<CommentType>", CommentType);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<BRComment> RetriveBRComment(string BRNum, Controller ctrl)
        {
            var ret = new List<BRComment>();
            var sql = "select BRNum,CommentKey,Comment,Reporter,CommentDate,CommentType from BRComment where BRNum = '<BRNum>' and Removed <> 'TRUE'";
            sql = sql.Replace("<BRNum>", BRNum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempcom = new BRComment();
                tempcom.BRNum = Convert.ToString(line[0]);
                tempcom.CommentKey = Convert.ToString(line[1]);
                tempcom.dbComment = Convert.ToString(line[2]);
                tempcom.Reporter = Convert.ToString(line[3]);
                tempcom.CommentDate = Convert.ToDateTime(line[4]);
                tempcom.CommentType = Convert.ToString(line[5]);
                ret.Add(tempcom);
            }
            return RepairBase64Image4IE(ret,ctrl);
        }

        private static string WriteBase64ImgFile(string commentcontent, Controller ctrl)
        {
            try
            {
                var idx = commentcontent.IndexOf("<img alt=\"\" src=\"data:image/png;base64");
                var base64idx = commentcontent.IndexOf("data:image/png;base64,", idx) + 22;
                var base64end = commentcontent.IndexOf("\"", base64idx);
                var imgstrend = commentcontent.IndexOf("/>", base64end) + 2;
                var base64img = commentcontent.Substring(base64idx, base64end - base64idx);
                var imgbytes = Convert.FromBase64String(base64img);

                var imgkey = Guid.NewGuid().ToString("N");
                string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\images\\" + datestring + "\\";

                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }
                var realpath = imgdir + imgkey + ".jpg";

                var fs = File.Create(realpath);
                fs.Write(imgbytes, 0, imgbytes.Length);
                fs.Close();


                var url = "/userfiles/images/" + datestring + "/" + imgkey + ".jpg";
                var ret = commentcontent;
                ret = ret.Remove(idx, imgstrend - idx);
                ret = ret.Insert(idx, "<img src='" + url + "'/>");

                return ret;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }


        private static string ReplaceBase64data2File(string commentcontent, Controller ctrl)
        {
            var ret = commentcontent;
            if (commentcontent.Contains("<img alt=\"\" src=\"data:image/png;base64"))
            {
                while (ret.Contains("<img alt=\"\" src=\"data:image/png;base64"))
                {
                    ret = WriteBase64ImgFile(ret, ctrl);
                    if (string.IsNullOrEmpty(ret))
                    {
                        ret = commentcontent;
                        break;
                    }
                }
            }

            return ret;
        }

        public static List<BRComment> RepairBase64Image4IE(List<BRComment> coments, Controller ctrl)
        {
            foreach (var com in coments)
            {
                var newcomment = ReplaceBase64data2File(com.Comment, ctrl);
                if (newcomment.Length != com.Comment.Length)
                {
                    com.Comment = newcomment;
                    UpdateBRComment(com.CommentKey, com.dbComment);
                }
            }
            return coments;
        }

        public static List<BRComment> RetriveBRCommentByKey(string CommentKey)
        {
            var ret = new List<BRComment>();
            var sql = "select BRNum,CommentKey,Comment,Reporter,CommentDate,CommentType from BRComment where CommentKey = '<CommentKey>' and Removed <> 'TRUE'";
            sql = sql.Replace("<CommentKey>", CommentKey);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempcom = new BRComment();
                tempcom.BRNum = Convert.ToString(line[0]);
                tempcom.CommentKey = Convert.ToString(line[1]);
                tempcom.dbComment = Convert.ToString(line[2]);
                tempcom.Reporter = Convert.ToString(line[3]);
                tempcom.CommentDate = Convert.ToDateTime(line[4]);
                tempcom.CommentType = Convert.ToString(line[5]);
                ret.Add(tempcom);
            }
            return ret;
        }

        public static void DeleteBRComment(string CommentKey)
        {
            var sql = "update BRComment set Removed = 'TRUE' where CommentKey='<CommentKey>'";
            sql = sql.Replace("<CommentKey>", CommentKey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateBRComment(string CommentKey, string dbcomment)
        {
            var csql = "update BRComment set Comment = '<Comment>'  where CommentKey='<CommentKey>'";
            csql = csql.Replace("<CommentKey>", CommentKey).Replace("<Comment>", dbcomment);
            DBUtility.ExeLocalSqlNoRes(csql);
        }

        public string ChangeType{set;get;}
        public string BRNumber{set;get;}
        public string Description{set;get;}
        public string Status{set;get;}
        public string Workflow{set;get;}
        public string Originator{set;get;}
        public DateTime OriginalDate{ set; get; }
        public string BRKey { set; get; }
        public string BRStatus { set; get; }
        public string ProjectKey { set; get; }

        public List<AgileWorkFlow> brworkflowlist{set;get;}
        public List<AgileAffectItem> affectitem{set;get;}
        public List<AgileHistory> history{set;get;}
        public List<AgileAttach> attach{set;get;}
    }
}