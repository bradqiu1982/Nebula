using Nebula.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nebula.Controllers
{
    public class BRTraceController : Controller
    {
        private void UserAuth()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser"))
            {
                ViewBag.EmailAddr = ckdict["logonuser"];
                ViewBag.UserName = ckdict["logonuser"].Replace("@FINISAR.COM", "");

                //ViewBag.IsPM
                ViewBag.IsPM = false;
                var syscfgdict = CfgUtility.GetSysConfig(this);
                var PMNames = syscfgdict["TRACEPM"];
                
                if(PMNames.ToUpper().Contains(ViewBag.UserName.ToUpper().Replace(".", " ")))
                {
                    ViewBag.IsPM = true;
                }
                
            }
        }

        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch (Exception ex)
            { return string.Empty; }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public ActionResult Home(string p,string smartkey = null)
        {
            if (smartkey != null)
            {
                var smartkey1 = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(smartkey));
                if (smartkey1.Contains("::"))
                {
                    var splitstr = smartkey1.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    var hash1 = splitstr[0];
                    var timestamp = splitstr[1];
                    MD5 md5Hash = MD5.Create();
                    var hash2 = GetMd5Hash(md5Hash, timestamp + "_joke");
                    if (hash1.Contains(hash2))
                    {
                        var now = DateTime.Now;
                        try
                        {
                            var time1 = DateTime.Parse(timestamp);
                            if (time1 > now.AddSeconds(-10))
                            {
                                //time is ok
                            }
                            else
                            {
                                return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                            }
                        }
                        catch (Exception ex) { return Redirect("http://wuxinpi.china.ads.finisar.com:8081/"); }

                    }
                    else
                    {
                        return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                    }
                }
                else
                {
                    return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                }
            }
            else if (Request.Cookies["activenpiNebula"] == null && smartkey == null)
            {
                string IP = Request.UserHostName;
                string compName = DetermineCompName(IP).ToUpper();
                var machinedict = CfgUtility.GetNPIMachine(this);
                if (!string.IsNullOrEmpty(compName) && !machinedict.ContainsKey(compName))
                {
                    return Redirect("http://wuxinpi.china.ads.finisar.com:8081/");
                }
            }

            var ckdict = CookieUtility.UnpackCookie(this);
            if (!ckdict.ContainsKey("logonuser") || string.IsNullOrEmpty(ckdict["logonuser"]))
            {
                return RedirectToAction("UserLogin", "NebulaUser");
            }

            UserAuth();

            int intp = 1;
            if (!string.IsNullOrEmpty(p))
            {
                intp = Convert.ToInt32(p);
            }

            var reviewer = (ViewBag.IsPM) ? ViewBag.UserName.Replace(".", " ") : null;
            List<BRAgileBaseInfo> allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(reviewer, BRJOSYSTEMSTATUS.OPEN,this);
            List<JOBaseInfo> allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(reviewer, BRJOSYSTEMSTATUS.OPEN);
            var page_size = 10;
            ViewBag.brlist = allBrlist.Skip((intp - 1) * page_size).Take(page_size);
            ViewBag.page = intp;
            ViewBag.total_pages = allBrlist.Count / page_size + 1;
            ViewBag.brlist_count = allBrlist.Count;
            ViewBag.jolist_count = allJolist.Count;

            return View();
        }

        public ActionResult DefaultBRList(int p = 1,string Status = null)
        {
            UserAuth();

            var allBrlist = new List<BRAgileBaseInfo>();
            Status = string.IsNullOrEmpty(Status) ? BRJOSYSTEMSTATUS.OPEN : Status;

            var reviewer = (ViewBag.IsPM) ? ViewBag.UserName : null;
            allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(reviewer, Status,this);
            ViewBag.withstatus = Status;

            var page_size = 10;
            ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allBrlist.Count / page_size + 1;
            ViewBag.searchkeyword = "";

            return View("BRList");
        }

        public ActionResult DefaultJOList(int p = 1, string Status = null)
        {
            UserAuth();
            var allJolist = new List<JOBaseInfo>();
            Status = string.IsNullOrEmpty(Status) ? BRJOSYSTEMSTATUS.OPEN : Status;

            var reviewer = (ViewBag.IsPM) ? ViewBag.UserName : null;
            allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(reviewer, Status);
            ViewBag.withstatus = Status;

            var page_size = 10;
            ViewBag.jolist = allJolist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allJolist.Count / page_size + 1;
            ViewBag.searchkeyword = "";

            return View("JOList");
        }

        public ActionResult JOList(string BR, int p = 1)
        {
            UserAuth();
            
            List<JOBaseInfo> allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null, null, BR);
            ViewBag.withstatus = string.Empty;

            var page_size = 10;
            ViewBag.jolist = allJolist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allJolist.Count / page_size + 1;
            ViewBag.searchkeyword = "";
            return View("JOList");
        }

        public ActionResult SearchKeyWord(string SearchWords, int p = 1)
        {
            UserAuth();

            var page_size = 10;
            ViewBag.page = p;
            ViewBag.searchkeyword = SearchWords;
            var allBrlist  = BRAgileBaseInfo.RetrieveBRAgileInfo(SearchWords,this);
            if (allBrlist.Count > 0)
            {
                ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
                ViewBag.total_pages = allBrlist.Count / page_size + 1;
                ViewBag.withstatus = "";
                return View("BRList");
            }
            else
            {
                var allJolist = JOBaseInfo.RetrieveJoInfo(SearchWords);
                if (allJolist.Count > 0)
                {
                    ViewBag.jolist = allJolist.Skip((p - 1) * page_size).Take(page_size);
                    ViewBag.total_pages = allJolist.Count / page_size + 1;
                    ViewBag.withstatus = "";
                    return View("JOList");
                }
                else
                {
                    return RedirectToAction("Home", "BRTrace");
                }
            }
        }



        public JsonResult BRAgileData()
        {
            var brnum = Request.Form["br_no"];

            var brinfolist = BRAgileBaseInfo.RetrieveBRAgileInfo(brnum,this);
            if (brinfolist.Count > 0)
            {
                var res = new JsonResult();
                res.Data = new { success = true
                    , PJKey = brinfolist[0].ProjectKey
                    , Originator = brinfolist[0].Originator
                    , OriginalDate = brinfolist[0].OriginalDate.ToString("yyyy-MM-dd HH:mm:ss")
                    , Description = brinfolist[0].Description
                    , Status = brinfolist[0].Status
                    , StartQTY = (brinfolist[0].pagethreelist.Count > 0) ? brinfolist[0].pagethreelist[0].startqty : ""
                    , TotalCost = (brinfolist[0].pagethreelist.Count > 0) ? brinfolist[0].pagethreelist[0].totalcost : ""
                    , ScrapQTY = (brinfolist[0].pagethreelist.Count > 0) ? brinfolist[0].pagethreelist[0].scrapqty : ""
                    , SaleRevenue = (brinfolist[0].pagethreelist.Count > 0) ? brinfolist[0].pagethreelist[0].salerevenue : ""
                    , ProductPhase = (brinfolist[0].pagethreelist.Count > 0) ? brinfolist[0].pagethreelist[0].productphase : ""
                };
                return res;
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false};
                return res;
            }

        }

        public JsonResult BRJOData()
        {
            var jonum = Request.Form["jo_no"];
            var jolist = JOBaseInfo.RetrieveJoInfo(jonum);
            if (jolist.Count > 0)
            {
                var res = new JsonResult();
                res.Data = new
                {
                    success = true,
                    pn = jolist[0].PN + "-" + jolist[0].PNDesc,
                    pnyd = jolist[0].PNYieldStr,
                    jotype = jolist[0].Category + "-" + jolist[0].JOType,
                    jostat = jolist[0].JOStatus,
                    jodate = jolist[0].DateReleased.ToString("yyyy-MM-dd HH:mm:ss"),
                    jowip = jolist[0].MRPNetQuantity.ToString(),
                    jostore = jolist[0].ExistQty.ToString(),
                    joplanner = jolist[0].Planner,
                    jopjkey = jolist[0].ProjectKey
                };
                return res;
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false };
                return res;
            }
        }

        public ActionResult BRInfo(string BRNum, string SearchWords, int p = 1, int sp = 1)
        {
            UserAuth();
            var reviewer = (ViewBag.IsPM) ? ViewBag.UserName : null;
            ViewBag.currentbr = BRAgileBaseInfo.RetrieveBRAgileInfo(BRNum,this)[0];
            List<JOBaseInfo> allcurrentbrjolist = JOBaseInfo.RetrieveJoInfoByBRNum(BRNum);
            var allsearchlist = new List<BRAgileBaseInfo>();
            if (SearchWords == "")
            {
                allsearchlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(reviewer, ViewBag.currentbr.BRStatus, this);
            }
            else {
                allsearchlist = BRAgileBaseInfo.RetrieveBRAgileInfo(SearchWords, this);
            }
            //br pagination
            var page_size = 10;
            ViewBag.currentsearchlist = allsearchlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allsearchlist.Count() / page_size + 1;


            //jo pagination
            var sub_page_size = 5;
            ViewBag.currentbrjolist = allcurrentbrjolist.Skip((sp - 1) * sub_page_size).Take(sub_page_size);
            ViewBag.sub_page = sp;
            ViewBag.sub_total_pages = allcurrentbrjolist.Count() / sub_page_size + 1;
            ViewBag.SearchWords = SearchWords;

            return View();
        }

        public ActionResult JOInfo(string JONum)
        {
            UserAuth();

            var jolist = JOBaseInfo.RetrieveJoInfo(JONum);
            return View(jolist[0]);
        }

        public void LoadNewBR()
        {
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            if (!Directory.Exists(datefolder))
            {
                Directory.CreateDirectory(datefolder);
            }

            var updatedfolder = datefolder + "agileupdated";
            var newqueryfolder = datefolder + "agilenewqueried";

            if (Directory.Exists(updatedfolder) 
                && !Directory.Exists(newqueryfolder))
            {
                var syscfgdict = CfgUtility.GetSysConfig(this);
                var AGILEURL = syscfgdict["AGILEURL"];
                var LOCALSITEPORT = syscfgdict["LOCALSITEPORT"];
                var SAVELOCATION = (Server.MapPath("~/userfiles") + "\\docs\\Agile\\").Replace("\\", "/");
                Directory.CreateDirectory(SAVELOCATION);
                var PMNames = syscfgdict["TRACEPM"];
                var FirstTraceTime = syscfgdict["FIRSTTRACETIME"];

                AgileDownloadVM.RetrieveNewBR(AGILEURL, LOCALSITEPORT, SAVELOCATION, PMNames, FirstTraceTime);
            }
            
        }

        public void UpdateExistBR()
        {
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            if (!Directory.Exists(datefolder))
            {
                Directory.CreateDirectory(datefolder);

                var syscfgdict = CfgUtility.GetSysConfig(this);
                var AGILEURL = syscfgdict["AGILEURL"];
                var LOCALSITEPORT = syscfgdict["LOCALSITEPORT"];
                var SAVELOCATION = (Server.MapPath("~/userfiles") + "\\docs\\Agile\\").Replace("\\", "/");
                Directory.CreateDirectory(SAVELOCATION);
                AgileDownloadVM.UpdateExistBR(AGILEURL, LOCALSITEPORT, SAVELOCATION);
            }
        }

        private void logthdinfo(string info)
        {
            try
            {
                var fn = "log" + DateTime.Now.ToString("yyyy-MM-dd");
                var filename = Server.MapPath("~/userfiles") + "\\" + fn;
                if (System.IO.File.Exists(filename))
                {
                    var content = System.IO.File.ReadAllText(filename);
                    content = content + "\r\n" + DateTime.Now.ToString() + " : " + info;
                    System.IO.File.WriteAllText(filename, content);
                }
                else
                {
                    System.IO.File.WriteAllText(filename, DateTime.Now.ToString() + " : " + info);
                }
            }
            catch (Exception ex)
            { }
        }

        public ActionResult HeartBeat()
        {
            UserAuth();

            logthdinfo("heart beat start");

            UpdateExistBR();
            LoadNewBR();

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            string newbrloaded = datefolder + "agilenewqueried";
            if (Directory.Exists(newbrloaded))
            {
                try
                {
                    logthdinfo("ERPVM.LoadJOBaseInfo");
                    ERPVM.LoadJOBaseInfo(this);
                }
                catch (Exception ex) { logthdinfo("load ERPVM.LoadJOBaseInfo exception:" + ex.Message); }
                try
                {
                    logthdinfo("ERPVM.LoadJOComponentInfo");
                    ERPVM.LoadJOComponentInfo(this);
                }
                catch (Exception ex) { logthdinfo("load ERPVM.LoadJOComponentInfo exception:" + ex.Message); }
                try
                {
                    logthdinfo("CamstarVM.UpdatePNWorkflow");
                    CamstarVM.UpdatePNWorkflow();
                }
                catch (Exception ex) { logthdinfo("load CamstarVM.UpdatePNWorkflow exception:" + ex.Message); }
                try
                {
                    logthdinfo("CamstarVM.UpdateJoMESStatus");
                    CamstarVM.UpdateJoMESStatus();
                }
                catch (Exception ex) { logthdinfo("load CamstarVM.UpdateJoMESStatus exception:" + ex.Message); }
                try
                {
                    logthdinfo("ERPVM.LoadJOShipTraceInfo");
                    ERPVM.LoadJOShipTraceInfo(this);
                }
                catch (Exception ex) { logthdinfo("load ERPVM.LoadJOShipTraceInfo exception:" + ex.Message); }
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                datestring = DateTime.Now.ToString("yyyyMMdd");
                datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
                var weeklyrepotdir = datefolder + "weeklyreport";
                if (!Directory.Exists(weeklyrepotdir)) {
                    Directory.CreateDirectory(weeklyrepotdir);
                    SendWeeklySBRReport();
                }

            }

            logthdinfo("heart beat end");

            return View();
        }

        private void SendWeeklySBRReport()
        {
            try
            {
                var cfg = CfgUtility.GetSysConfig(this);
                var to = cfg["REPORTRECIEVER"];
                var tolist = new List<string>();
                tolist.AddRange(to.Split(new string[] { ";"},StringSplitOptions.RemoveEmptyEntries));

                var htmltablelist = BRReportVM.RetrieveActiveBRRpt(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd ")+"00:00:01", DateTime.Now.ToString("yyyy-MM-dd ")+"23:59:00");
                if (htmltablelist.Count > 0)
                {
                    var content = EmailUtility.CreateTableHtml2("Hi guys", "Below is an SBR report of WUXI NPI:", "", htmltablelist);
                    EmailUtility.SendEmail(this, "WUXI NPI SBR Report", tolist, content,true);
                    new System.Threading.ManualResetEvent(false).WaitOne(500);
                }
            }
            catch (Exception ex) { }
        }

        public ActionResult HeartBeat2()
        {
            SendWeeklySBRReport();
            return View("HeartBeat");
        }

        private void CreateAgileDir(string detaildir)
        {
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

            if (!Directory.Exists(datefolder))
            {
                Directory.CreateDirectory(datefolder);
            }

            var newqueryfolder = datefolder + detaildir;
            if (!Directory.Exists(newqueryfolder))
            {
                Directory.CreateDirectory(newqueryfolder);
            }
        }

        public ActionResult NewBR(string BRLIST)
        {
            UserAuth();

            BRAgileVM.LoadNewBR(BRLIST, this);
            CreateAgileDir("agilenewqueried");
            return View();
        }

        public ActionResult UpdateBR()
        {
            UserAuth();

            BRAgileVM.UpdateBR(this);
            CreateAgileDir("agileupdated");
            return View();
        }

        public ActionResult JODetail(string BRNum, string JONum, int Step)
        {
            UserAuth();
            ViewBag.BRNum = BRNum;
            ViewBag.JONum = JONum;
            ViewBag.Step = Step;

            ViewBag.ProjectKey = "";

            var titlelist = new List<string>();
            titlelist.Add("Please select workflow");
            var jobaseinfo = JOBaseInfo.RetrieveJoInfo(JONum);
            if (jobaseinfo.Count > 0 && !string.IsNullOrEmpty(jobaseinfo[0].PN))
            {
                var flowlist = PNWorkflow.RetrievePNWorkflow(jobaseinfo[0].PN);
                foreach (var flow in flowlist)
                {
                    titlelist.Add(flow.WorkflowStepName);
                }

                ViewBag.ProjectKey = jobaseinfo[0].ProjectKey;
            }

            var titlectrl = CreateSelectList(titlelist, "");
            titlectrl[0].Selected = true;
            titlectrl[0].Disabled = true;
            ViewBag.titlelist = titlectrl;

            ViewBag.CurrentDay = JOScheduleEventDataVM.RetriveLatestScheduleDate(JONum);

            return View();
        }


        private List<SelectListItem> CreateSelectList(List<string> valist, string defVal)
        {
            bool selected = false;
            var pslist = new List<SelectListItem>();
            foreach (var p in valist)
            {
                var pitem = new SelectListItem();
                pitem.Text = p;
                pitem.Value = p;
                if (!string.IsNullOrEmpty(defVal) && string.Compare(defVal, p, true) == 0)
                {
                    pitem.Selected = true;
                    selected = true;
                }
                pslist.Add(pitem);
            }

            if (!selected && pslist.Count > 0)
            {
                pslist[0].Selected = true;
            }

            return pslist;
        }


        public JsonResult JOSchedules()
        {
            var jonum = Request.Form["JoNum"];
            var list = JOScheduleEventDataVM.RetrieveScheduleByJoNum(jonum);
            //var list = new List<JOScheduleEventDataVM>();
            //list.Add(new JOScheduleEventDataVM("","abc","hello world", "bg-success border-transparent","","2017-09-11 11:00:00", "2017-09-13 11:00:00"));
            //list.Add(new JOScheduleEventDataVM("", "efg", "hello world 2", "bg-primary,border-primary-dark", "", "2017-09-15 11:00:00", "2017-09-19 11:00:00"));
            var res = new JsonResult();
            res.Data = list;
            return res;
        }

        private static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private JOScheduleEventDataVM ParseEventJason()
        {
            var ret = new JOScheduleEventDataVM();

            if (Request.Form["id"] != null)
            {
                ret.id = Request.Form["id"];
            }
            if (Request.Form["title"] != null)
            {
                ret.title = Request.Form["title"];
            }
            if (Request.Form["className[]"] != null)
            {
                ret.className = Request.Form["className[]"].Replace(",", " ");
            }
            if (Request.Form["description"] != null)
            {
                ret.desc = Request.Form["description"];
            }

            if (Request.Form["start"] != null)
            {
                ret.start = Request.Form["start"];
            }

            if (Request.Form["end"] != null)
            {
                ret.end = Request.Form["end"];
            }

            if (Request.Form["jonum"] != null)
            {
                ret.jonum = Request.Form["jonum"];
            }
            
            return ret;
        }



        [HttpPost]
        public JsonResult AddScheduleEvent()
        {
            var vm = ParseEventJason();
            var eventid = GetUniqKey();
            if (!string.IsNullOrEmpty(vm.title)
                && !string.IsNullOrEmpty(vm.jonum)
                && !string.IsNullOrEmpty(vm.start)
                && !string.IsNullOrEmpty(vm.end))
            {
                vm.id = eventid;
                var ret = vm.AddSchedule();

                if (ret)
                {
                    var res = new JsonResult();
                    res.Data = new { success = true, id = eventid };
                    return res;
                }
                else
                {
                    var res = new JsonResult();
                    res.Data = new { success = false, msg = "same workflow schedule has already been added" };
                    return res;
                }
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "uploaded schedule data is empty" };
                return res;
            }
        }

        public JsonResult UpdateScheduleEvent()
        {
            var vm = ParseEventJason();
            if (!string.IsNullOrEmpty(vm.title)
                && !string.IsNullOrEmpty(vm.id)
                && !string.IsNullOrEmpty(vm.start)
                && !string.IsNullOrEmpty(vm.end))
            {
                var ret = vm.UpdateSchedule();
                if (ret)
                {
                    var res = new JsonResult();
                    res.Data = new { success = true };
                    return res;
                }
                else
                {
                    var res = new JsonResult();
                    res.Data = new { success = false, msg = "same workflow schedule has already existed or no such schedule" };
                    return res;
                }
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "uploaded schedule data is empty" };
                return res;
            }

        }

        public JsonResult RemoveScheduleEvent()
        {
            var vm = ParseEventJason();
            if (!string.IsNullOrEmpty(vm.id))
            {
                vm.RemoveSchedule(vm.id);
                var res = new JsonResult();
                res.Data = new { success = true };
                return res;
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "uploaded schedule data is empty" };
                return res;
            }

        }

        public JsonResult MoveScheduleEvent()
        {
            var vm = ParseEventJason();
            if (!string.IsNullOrEmpty(vm.id)
                && !string.IsNullOrEmpty(vm.start)
                && !string.IsNullOrEmpty(vm.end))
            {
                var ret = vm.MoveSchedule();
                if (ret)
                {
                    var res = new JsonResult();
                    res.Data = new { success = true };
                    return res;
                }
                else
                {
                    var res = new JsonResult();
                    res.Data = new { success = false, msg = "such schedule does not exist" };
                    return res;
                }
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "uploaded schedule data is empty" };
                return res;
            }
        }


        public JsonResult JoDistributionData()
        {
            var jonum = Request.Form["JONum"];
            var josnstatlist = JOSNStatus.RetrieveJOSNStatus(jonum);
            var list = JOSNonStation.RetrieveJOSNStation(josnstatlist);

            var res = new JsonResult();
            res.Data = list;
            return res;
        }

        private List<PNErrorPareto> RetrievePNPareto(string PN)
        {
                var pnerrdist = JOBaseInfo.RetrievePNErrorSum(PN);
                pnerrdist.Sort(delegate (PNErrorDistribute pair1, PNErrorDistribute pair2)
                {
                    return pair2.Amount.CompareTo(pair1.Amount);
                });

                var sum = 0;
                for (var i = 0; i < pnerrdist.Count; i++)
                {
                    sum = sum + pnerrdist[i].Amount;
                }

                var peralist = new List<PNErrorPareto>();
                var otherpercent = 0.0;

                for (var i = 0; i < pnerrdist.Count; i++)
                {
                    if (pnerrdist.Count > 5 && peralist.Count > 0 && Convert.ToDouble(peralist[peralist.Count - 1].PPercent) > 95.0)
                    {
                        otherpercent = otherpercent + pnerrdist[i].Amount / (double)sum;
                        if (i == (pnerrdist.Count - 1))
                        {
                            var tempperato = new PNErrorPareto();
                            tempperato.Failure = "Other";
                            tempperato.Amount = (int)(otherpercent * sum);
                            tempperato.ABPercent = otherpercent * 100.0;
                            tempperato.PPercent = 100.0;
                            peralist.Add(tempperato);
                        }
                    }
                    else
                    {
                        var tempperato = new PNErrorPareto();
                        tempperato.Failure = pnerrdist[i].ErrAttr;
                        if (i == 0)
                        {
                            tempperato.Amount = pnerrdist[i].Amount;
                            tempperato.ABPercent = (pnerrdist[i].Amount / (double)sum) * 100.0;
                            tempperato.PPercent = tempperato.ABPercent;
                            peralist.Add(tempperato);
                        }
                        else
                        {
                            tempperato.Amount = pnerrdist[i].Amount;
                            tempperato.ABPercent = (pnerrdist[i].Amount / (double)sum) * 100.0;
                            tempperato.PPercent = Convert.ToDouble(peralist[peralist.Count - 1].PPercent) + Convert.ToDouble(tempperato.ABPercent);
                            peralist.Add(tempperato);
                        }
                    }
                }
            return peralist;
        }

        public JsonResult PNErrorDistribution()
        {
            var jonum = Request.Form["JONum"];
            var joinfo = JOBaseInfo.RetrieveJoInfo(jonum);
            var list = new List<PNErrorPareto>();

            if (joinfo.Count > 0)
            {
                list = RetrievePNPareto(joinfo[0].PN);
            }

            var res = new JsonResult();
            res.Data = list;
            return res;
        }

        public ActionResult JOComponent(string JONum)
        {
            UserAuth();
            var jocomps = JOComponentInfo.RetrieveInfo(JONum);
            return View(jocomps);
        }

        public ActionResult JOShipping(string JONum)
        {
            var joship = JOShipTrace.RetireTraceInfo(JONum);
            return View(joship);
        }


        [HttpPost]
        public ActionResult AddBRComment()
        {
            UserAuth();
            if (string.IsNullOrEmpty(ViewBag.UserName))
            {
                return RedirectToAction("Home", "BRTrace");
            }

            var brnum = Request.Form["HBRNUM"];
            var SearchWords = Request.Form["SearchWords"];
            if (!string.IsNullOrEmpty(brnum))
            {
                if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
                {
                    var generalcomment = SeverHtmlDecode.Decode(this, Request.Form["commenteditor"]);
                    var brcomment = new BRComment();
                    brcomment.Comment = generalcomment;
                    var commentid = Request.Form["commentid"];
                    if (string.IsNullOrEmpty(commentid))
                    {
                        BRAgileBaseInfo.StoreBRComment(brnum, brcomment.dbComment, BRCOMMENTTP.COMMENT, ViewBag.UserName);
                    }
                    else
                    {
                        BRAgileBaseInfo.UpdateBRComment(commentid, brcomment.dbComment);
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["conclusioneditor"]))
                {
                    var conclusioncomment = SeverHtmlDecode.Decode(this, Request.Form["conclusioneditor"]);
                    var brcomment = new BRComment();
                    brcomment.Comment = conclusioncomment;

                    var conclusionid = Request.Form["conclusionid"];
                    if (string.IsNullOrEmpty(conclusionid))
                    {
                        BRAgileBaseInfo.StoreBRComment(brnum, brcomment.dbComment, BRCOMMENTTP.CONCLUSION, ViewBag.UserName);
                    }
                    else
                    {
                        BRAgileBaseInfo.UpdateBRComment(conclusionid, brcomment.dbComment);
                    }
                    
                }

                var dict1 = new RouteValueDictionary();
                dict1.Add("BRNum", brnum);
                dict1.Add("SearchWords", SearchWords);
                return RedirectToAction("BRInfo", "BRTrace", dict1);
            }
            else
            {
                return RedirectToAction("Home", "BRTrace");
            }
        }

        public ActionResult DeleteBRComment(string CommentKey, string BRNum, string SearchWords)
        {
            BRAgileBaseInfo.DeleteBRComment(CommentKey);
            var dict1 = new RouteValueDictionary();
            dict1.Add("BRNum", BRNum);
            dict1.Add("SearchWords", SearchWords);
            return RedirectToAction("BRInfo", "BRTrace", dict1);
        }


        public JsonResult ChangeTheme()
        {
            string IP = Request.UserHostName;
            string machine = DetermineCompName(IP).ToUpper();
            int theme = Theme.Dark;
            if (Session["utheme"] == null || Convert.ToInt32(Session["utheme"]) == Theme.Dark)
            {
                theme = Theme.Light;
            }
            var ctheme = new UserCustomizeThemeVM(
                0,
                machine,
                Convert.ToInt32(theme),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            );
            UserCustomizeThemeVM.UpdateTheme(ctheme);
            //Session.Remove("utheme");
            Session.Add("utheme", theme.ToString());
            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        private string  CreateTabelHtml(List<List<string>> table)
        {
            var content = string.Empty;
            var idx = 0;
            if (table != null)
            {
                content += "<div  style='color: #fff; margin-right: 10%; margin-left: 10%;'><br>";
                content += "<table border-radius='5px' cellpadding='0' cellspacing='0' width='100%' style='border-radius: 10px; border: 1px solid rgb(204, 204, 204); border-image: none; border-collapse: separate; -webkit-border-radius: 10px;'>";
                content += "<colgroup><col style='width:20%;'><col style='width:60%;'><col style='width:20%;'></colgroup>";
                content += "<thead color: #fff;'>";
                foreach (var th in table[0])
                {
                    content += "<th>" + th + "</th>";
                }
                content += "</thead>";
                foreach (var tr in table)
                {
                    if (idx > 0)
                    {
                        var tidx = 0;
                        content += "<tr>";
                        foreach (var td in tr)
                        {
                            if (tidx == 0)
                            {
                                content += "<td><strong>" + td + "</strong></td>";
                            }
                            else
                            {
                                content += "<td>" + td + "</td>";
                            }
                            tidx++;
                        }
                        content += "</tr>";
                    }
                    idx++;
                }
                content += "</table>";
                content += "</div>";

                return content;
            }

            return string.Empty;
        }

        public ActionResult BRReport(string PM,int Weeks = 1)
        {
            var endtime = DateTime.Now;
            var starttime = DateTime.Now.AddDays(-7 * Weeks);
            var filterbr = new List<BRReportVM>();
            var brlist = BRReportVM.RetrieveActiveBRRptVM(starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            foreach (var br in brlist)
            {
                if (!string.IsNullOrEmpty(PM))
                {
                    if (string.Compare(br.Originator.Replace(".", "").Replace(" ", "").ToUpper()
                        , PM.Replace(".", "").Replace(" ", "").ToUpper()) == 0)
                    {
                        filterbr.Add(br);
                    }
                }
                else
                {
                    filterbr.Add(br);
                }
            }

            var tablist = new List<string>();
            foreach (var br in filterbr)
            {
                var temptablist = BRReportVM.BRReportToArray(br);
                tablist.Add(CreateTabelHtml(temptablist));
            }

            ViewBag.HPM = "";
            if (!string.IsNullOrEmpty(PM))
            {
                ViewBag.HPM = PM;
            }
            ViewBag.HWeeks = Weeks.ToString();
            ViewBag.tablelist = tablist;

            var pmlist = BRReportVM.RetrievePMList();
            var templist = new List<string>();
            templist.Add("PM LIST (Optional)");
            templist.AddRange(pmlist);
            ViewBag.pmselectlist = CreateSelectList(templist, PM);
            if (string.IsNullOrEmpty(PM))
            {
                ViewBag.pmselectlist[0].Selected = true;
                ViewBag.pmselectlist[0].Disabled = true;
            }

            templist = new List<string>();
            templist.AddRange(new string[] { "1", "2", "4", "8", "16", "32", "48" });
            ViewBag.weeksselectlist = CreateSelectList(templist, Weeks.ToString());

            return View();
        }

        public JsonResult SendBRReport()
        {
            UserAuth();

            if (string.IsNullOrEmpty(ViewBag.EmailAddr))
            {
                var ret = new JsonResult();
                ret.Data = new { msg = "Fail to get your email, please login first!" };
                return ret;
            }
            else
            {
                var Weeks = Convert.ToInt32(Request.Form["weeks"]);
                var PM = Request.Form["pm"];

                var endtime = DateTime.Now;
                var starttime = DateTime.Now.AddDays(-7 * Weeks);
                var filterbr = new List<BRReportVM>();
                var brlist = BRReportVM.RetrieveActiveBRRptVM(starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                foreach (var br in brlist)
                {
                    if (!string.IsNullOrEmpty(PM))
                    {
                        if (string.Compare(br.Originator.Replace(".", "").Replace(" ", "").ToUpper()
                            , PM.Replace(".", "").Replace(" ", "").ToUpper()) == 0)
                        {
                            filterbr.Add(br);
                        }
                    }
                    else
                    {
                        filterbr.Add(br);
                    }
                }

                if (filterbr.Count > 0)
                {
                    var tolist = new List<string>();
                    tolist.Add(ViewBag.EmailAddr);
                    var htmltablelist = new List<string>();
                    foreach (var br in filterbr)
                    {
                        var temptablist =BRReportVM.BRReportToArray(br);
                        htmltablelist.Add(EmailUtility.CreateTableStr(temptablist));
                    }

                    if (htmltablelist.Count > 0)
                    {
                        var title = "Below is an SBR report of WUXI NPI in " + Weeks.ToString() + " weeks";
                        if (!string.IsNullOrEmpty(PM))
                        {
                            title = title + " filtered with PM-" + PM;
                        }
                        title = title + ":";

                        var content = EmailUtility.CreateTableHtml2("Hi guys",title , "", htmltablelist);
                        EmailUtility.SendEmail(this, "WUXI NPI SBR Report", tolist, content, true);
                        new System.Threading.ManualResetEvent(false).WaitOne(500);
                    }

                    var ret = new JsonResult();
                    ret.Data = new { msg = "BR report is send,please check your email!" };
                    return ret;
                }
                else
                {
                    var ret = new JsonResult();
                    ret.Data = new { msg = "No BR information is selected!" };
                    return ret;
                }
            }
        }


    }


}