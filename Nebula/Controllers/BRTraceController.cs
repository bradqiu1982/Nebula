using Nebula.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            }
        }

        public ActionResult Home(int p = 1)
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!ckdict.ContainsKey("logonuser") || string.IsNullOrEmpty(ckdict["logonuser"]))
            {
                return RedirectToAction("UserLogin", "NebulaUser");
            }

            UserAuth();

            var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo(null);
            var allJolist = JOBaseInfo.RetrieveActiveJoInfo(null);
            var page_size = 10;
            ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allBrlist.Count / page_size + 1;
            ViewBag.brlist_count = allBrlist.Count;
            ViewBag.jolist_count = allJolist.Count;

            return View();
        }

        public ActionResult DefaultBRList(int p = 1)
        {

            UserAuth();
            var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo(null);
            
            var page_size = 10;
            ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allBrlist.Count / page_size + 1;
            ViewBag.searchkeyword = "";

            return View("BRList");
        }

        public ActionResult DefaultJOList(int p = 1)
        {
            UserAuth();

            var allJolist = JOBaseInfo.RetrieveActiveJoInfo(null);
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
            var allBrlist  = BRAgileBaseInfo.RetrieveBRAgileInfo(SearchWords);
            if (allBrlist.Count > 0)
            {
                ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
                ViewBag.total_pages = allBrlist.Count / page_size + 1;
                return View("BRList");
            }
            else
            {
                var allJolist = JOBaseInfo.RetrieveJoInfo(SearchWords);
                if (allJolist.Count > 0)
                {
                    ViewBag.jolist = allJolist.Skip((p - 1) * page_size).Take(page_size);
                    ViewBag.total_pages = allJolist.Count / page_size + 1;
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

            var brinfolist = BRAgileBaseInfo.RetrieveBRAgileInfo(brnum);
            if (brinfolist.Count > 0)
            {
                var res = new JsonResult();
                res.Data = new { success = true
                    , Originator = brinfolist[0].Originator
                    , OriginalDate = brinfolist[0].OriginalDate.ToString("yyyy-MM-dd hh:mm:ss")
                    , Description = brinfolist[0].Description
                    , Status = brinfolist[0].Status
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
                    jotype = jolist[0].Category+"-"+jolist[0].JOType,
                    jostat = jolist[0].JOStatus,
                    jodate = jolist[0].DateReleased.ToString("yyyy-MM-dd hh:mm:ss"),
                    jowip = jolist[0].WIP.ToString(),
                    joplanner = jolist[0].Planner
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

            ViewBag.currentbr = BRAgileBaseInfo.RetrieveBRAgileInfo(BRNum)[0];
            var allcurrentbrjolist = JOBaseInfo.RetrieveJoInfoByBRNum(BRNum);
            var allsearchlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo((!string.IsNullOrEmpty(SearchWords))? SearchWords : null);
            //br pagination
            var page_size = 10;
            ViewBag.currentsearchlist = allsearchlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allsearchlist.Count / page_size + 1;

            //jo pagination
            var sub_page_size = 5;
            ViewBag.currentbrjolist = allcurrentbrjolist.Skip((sp - 1) * sub_page_size).Take(sub_page_size);
            ViewBag.sub_page = sp;
            ViewBag.sub_total_pages = allcurrentbrjolist.Count / sub_page_size + 1;

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

        public ActionResult HeartBeat()
        {
            UserAuth();

            //UpdateExistBR();
            //LoadNewBR();
            //ERPVM.LoadJOBaseInfo(this);
            //ERPVM.LoadJOComponentInfo(this);
            //CamstarVM.UpdatePNWorkflow();
            CamstarVM.UpdateJoStatus();
            return View();
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

            var titlelist = new List<string>();
            titlelist.Add("Please select workflow");
            titlelist.Add("Die Attach");
            titlelist.Add("Wire Bonding");
            titlelist.Add("Lens Alignment");
            titlelist.Add("HotBar");
            var titlectrl = CreateSelectList(titlelist, "");
            titlectrl[0].Selected = true;
            titlectrl[0].Disabled = true;
            ViewBag.titlelist = titlectrl;

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

        public ActionResult JoSchedule()
        {
            var titlelist = new List<string>();
            titlelist.Add("Please select workflow");
            titlelist.Add("Die Attach");
            titlelist.Add("Wire Bonding");
            titlelist.Add("Lens Alignment");
            titlelist.Add("HotBar");
            var titlectrl = CreateSelectList(titlelist, "");
            titlectrl[0].Selected = true;
            titlectrl[0].Disabled = true;
            ViewBag.titlelist = titlectrl;

            ViewBag.evenstrs = "{id:'abcd',title:'All Day Event',start:'2017-08-01'},{id:'efgh',title:'Long Event',start:'2017-08-07',end:'2015-09-10',className:'bg-success border-transparent'}";
            ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        public JsonResult JOSchedules()
        {
            var jonum = Request.Form["JoNum"];
            var list = new List<EventDataVM>();
            list.Add(new EventDataVM("abcd", "Hello world 1", "bg-success border-transparent", string.Empty, "2017-09-01", "2017-09-04"));
            list.Add(new EventDataVM("efgh", "my world", "bg-success border-transparent", string.Empty, "2017-09-05", "2017-09-10"));
            var res = new JsonResult();
            res.Data = list;
            return res;
        }

        private static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private EventDataVM ParseEventJason()
        {
            var ret = new EventDataVM();

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
                ret.className = Request.Form["className[]"];
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

            return ret;
        }



        [HttpPost]
        public JsonResult AddScheduleEvent()
        {
            var vm = ParseEventJason();
            var eventid = GetUniqKey();

            var res = new JsonResult();
            res.Data = new { success = true, id = eventid };
            return res;
        }

        public JsonResult UpdateScheduleEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        public JsonResult RemoveScheduleEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        public JsonResult MoveScheduleEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        public ActionResult JoDistribution()
        {
            return View();
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

    }
}