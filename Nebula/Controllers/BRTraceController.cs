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

            //var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo(null);
            //var allJolist = JOBaseInfo.RetrieveActiveJoInfo(null);
            var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, BRJOSYSTEMSTATUS.OPEN);
            var allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null,BRJOSYSTEMSTATUS.OPEN);
            var page_size = 10;
            ViewBag.brlist = allBrlist.Skip((p - 1) * page_size).Take(page_size);
            ViewBag.page = p;
            ViewBag.total_pages = allBrlist.Count / page_size + 1;
            ViewBag.brlist_count = allBrlist.Count;
            ViewBag.jolist_count = allJolist.Count;

            return View();
        }

        public ActionResult DefaultBRList(int p = 1,string Status = null)
        {
            UserAuth();

            var allBrlist = new List<BRAgileBaseInfo>();

            if (string.IsNullOrEmpty(Status))
            {
                allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, BRJOSYSTEMSTATUS.OPEN);
                ViewBag.withstatus = "FALSE";
            }
            else
            {
                allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, BRJOSYSTEMSTATUS.CLOSE);
                ViewBag.withstatus = "TRUE";
            }
            
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

            if (string.IsNullOrEmpty(Status))
            {
                allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null, BRJOSYSTEMSTATUS.OPEN);
                ViewBag.withstatus = "FALSE";
            }
            else
            {
                allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null, BRJOSYSTEMSTATUS.CLOSE);
                ViewBag.withstatus = "TRUE";
            }
            

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

            var brinfolist = BRAgileBaseInfo.RetrieveBRAgileInfo(brnum);
            if (brinfolist.Count > 0)
            {
                var res = new JsonResult();
                res.Data = new { success = true
                    , Originator = brinfolist[0].Originator
                    , OriginalDate = brinfolist[0].OriginalDate.ToString("yyyy-MM-dd HH:mm:ss")
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
                    jodate = jolist[0].DateReleased.ToString("yyyy-MM-dd HH:mm:ss"),
                    jowip = jolist[0].MRPNetQuantity.ToString(),
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
            var allcurrentbrjolist = (IEnumerable<JOBaseInfo>)JOBaseInfo.RetrieveJoInfoByBRNum(BRNum);
            var allsearchlist = (IEnumerable<BRAgileBaseInfo>)BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, ViewBag.currentbr.BRStatus);//BRAgileBaseInfo.RetrieveActiveBRAgileInfo((!string.IsNullOrEmpty(SearchWords))? SearchWords : null);
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

            UpdateExistBR();
            LoadNewBR();

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string datefolder = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            string newbrloaded = datefolder + "agilenewqueried";
            if (Directory.Exists(newbrloaded))
            {
                ERPVM.LoadJOBaseInfo(this);
                ERPVM.LoadJOComponentInfo(this);
                CamstarVM.UpdatePNWorkflow();
                CamstarVM.UpdateJoMESStatus();
            }


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
            var jobaseinfo = JOBaseInfo.RetrieveJoInfo(JONum);
            if (jobaseinfo.Count > 0 && !string.IsNullOrEmpty(jobaseinfo[0].PN))
            {
                var flowlist = PNWorkflow.RetrievePNWorkflow(jobaseinfo[0].PN);
                foreach (var flow in flowlist)
                {
                    titlelist.Add(flow.WorkflowStepName);
                }
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

    }
}