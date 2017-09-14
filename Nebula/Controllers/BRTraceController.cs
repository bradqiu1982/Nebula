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
        public ActionResult Home()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!ckdict.ContainsKey("logonuser"))
            {
                return RedirectToAction("UserLogin", "NebulaUser");
            }
            var loginer = ckdict["logonuser"];

            ViewBag.brlist = BRAgileBaseInfo.RetrieveActiveBRAgileInfo(null);
            ViewBag.jolist = JOBaseInfo.RetrieveActiveJoInfo(null);

            return View();
        }

        public ActionResult SearchKeyWord(string Keywords)
        {
            var brlist  = BRAgileBaseInfo.RetrieveBRAgileInfo(Keywords);
            if (brlist.Count > 0)
            {
                ViewBag.searchkeyword = Keywords;
                return View("BRList", brlist);
            }
            else
            {
                var jolist = JOBaseInfo.RetrieveJoInfo(Keywords);
                if (jolist.Count > 0)
                {
                    ViewBag.searchkeyword = Keywords;
                    return View("JOList", jolist);
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

        public ActionResult BRInfo(string BRNum, string SearchWords)
        {
            ViewBag.currentbr = BRAgileBaseInfo.RetrieveBRAgileInfo(BRNum)[0];
            ViewBag.currentbrjolist = JOBaseInfo.RetrieveJoInfoByBRNum(BRNum);
            ViewBag.currentsearchlist = BRAgileBaseInfo.RetrieveBRAgileInfo(SearchWords);
            ViewBag.searchkeyword = SearchWords;
            return View();
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
            BRAgileVM.LoadNewBR(BRLIST, this);
            CreateAgileDir("agilenewqueried");
            return View();
        }

        public ActionResult UpdateBR()
        {
            BRAgileVM.UpdateBR(this);
            CreateAgileDir("agileupdated");
            return View();
        }

        

    }
}