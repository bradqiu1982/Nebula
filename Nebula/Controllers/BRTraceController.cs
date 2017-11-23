﻿using Nebula.Models;
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

            //var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo(null);
            //var allJolist = JOBaseInfo.RetrieveActiveJoInfo(null);
            var allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, BRJOSYSTEMSTATUS.OPEN,this);
            var allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null,BRJOSYSTEMSTATUS.OPEN);
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

            if (string.IsNullOrEmpty(Status))
            {
                allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, BRJOSYSTEMSTATUS.OPEN,this);
                ViewBag.withstatus = BRJOSYSTEMSTATUS.OPEN;
            }
            else
            {
                allBrlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, Status,this);
                ViewBag.withstatus = Status;
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
                ViewBag.withstatus = BRJOSYSTEMSTATUS.OPEN;
            }
            else
            {
                allJolist = JOBaseInfo.RetrieveActiveJoInfoWithStatus(null, Status);
                ViewBag.withstatus = Status;
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

            ViewBag.currentbr = BRAgileBaseInfo.RetrieveBRAgileInfo(BRNum,this)[0];
            var allcurrentbrjolist = (IEnumerable<JOBaseInfo>)JOBaseInfo.RetrieveJoInfoByBRNum(BRNum);
            var allsearchlist = (IEnumerable<BRAgileBaseInfo>)BRAgileBaseInfo.RetrieveActiveBRAgileInfoWithStatus(null, ViewBag.currentbr.BRStatus,this);//BRAgileBaseInfo.RetrieveActiveBRAgileInfo((!string.IsNullOrEmpty(SearchWords))? SearchWords : null);
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
            var jocomps = JOComponentInfo.RetrieveInfo(JONum);
            return View(jocomps);
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
                dict1.Add("SearchWords", "");
                return RedirectToAction("BRInfo", "BRTrace", dict1);
            }
            else
            {
                return RedirectToAction("Home", "BRTrace");
            }
        }

        public ActionResult DeleteBRComment(string CommentKey, string BRNum)
        {
            BRAgileBaseInfo.DeleteBRComment(CommentKey);
            var dict1 = new RouteValueDictionary();
            dict1.Add("BRNum", BRNum);
            dict1.Add("SearchWords", "");
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

    }


}