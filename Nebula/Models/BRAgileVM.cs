using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Globalization;

namespace Nebula.Models
{
    public class BRAgileVM
    {
        private static List<BRAgileBaseInfo> ParseBR(string BRLIST,Controller ctrl)
        {
            var ret = new List<BRAgileBaseInfo>();
            var brs = BRLIST.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
            BRAgileBaseInfo tempinfo = null;
            foreach (var br in brs)
            {
                tempinfo = null;
                string brfile = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + br.Trim() + "\\" + br.Trim() + "-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
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

                    ret.Add(tempinfo);
                }
            }
            return ret;
        }

        public static void LoadNewBR(string BRLIST, Controller ctrl)
        {
            ParseBR(BRLIST, ctrl);
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
            itemnumber = "";
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
                    item.itemnumber = value;
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

        public string itemnumber{set;get;}
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
            user = "";
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
                    item.user = value;
                if (string.Compare(kv[0], "localtime", true) == 0)
                    item.localtime = BRAgileVM.ConvertUSLocalToDate(value);
                if (string.Compare(kv[0], "detail", true) == 0)
                    item.detail = value;
                if (string.Compare(kv[0], "usernotice", true) == 0)
                    item.usernotice = value;
            }
            return item;
        }

        public string status{set;get;}
        public string nextstatus{set;get;}
        public string action{set;get;}
        public string user{set;get;}
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

        public string FileName{set;get;}
        public DateTime ModifyDate{set;get;}
        public string Checkiner{set;get;}
        public string LocalFilePath{set;get;}
    }

    public class BRAgileBaseInfo
    {
        public BRAgileBaseInfo()
        {
            ChangeType = "";
            Number = "";
            Description = "";
            Status = "";
            Workflow = "";
            Originator = "";
            OriginalDate = DateTime.Parse("1982-05-06 10:00:00");
            brworkflowlist = new List<AgileWorkFlow>();
            affectitem = new List<AgileAffectItem>();
            history = new List<AgileHistory>();
            attach = new List<AgileAttach>();
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
                    item.Number = value;
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

        public string ChangeType{set;get;}
        public string Number{set;get;}
        public string Description{set;get;}
        public string Status{set;get;}
        public string Workflow{set;get;}
        public string Originator{set;get;}
        public DateTime OriginalDate{set;get;}

        public List<AgileWorkFlow> brworkflowlist{set;get;}
        public List<AgileAffectItem> affectitem{set;get;}
        public List<AgileHistory> history{set;get;}
        public List<AgileAttach> attach{set;get;}
    }
}