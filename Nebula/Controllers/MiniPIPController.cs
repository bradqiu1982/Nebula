using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nebula.Models;
using System.Reflection;
using System.Web.Routing;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Nebula.Controllers
{
    public class MiniPIPController : Controller
    {

        public ActionResult JoSchedule()
        {
            var titlelist = new List<string>();
            titlelist.Add("Please select workflow");
            titlelist.Add("Die Attach");
            titlelist.Add("Wire Bonding");
            titlelist.Add("Lens Alignment");
            titlelist.Add("HotBar");
            var titlectrl =  CreateSelectList(titlelist, "");
            titlectrl[0].Selected = true;
            titlectrl[0].Disabled = true;
            ViewBag.titlelist = titlectrl;

            ViewBag.evenstrs = "{id:'abcd',title:'All Day Event',start:'2017-09-01'},{id:'efgh',title:'Long Event',start:'2017-09-07',end:'2015-09-10',className:'bg-success border-transparent'}";
            ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        private static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        private EventDataVM ParseEventJason()
        {
            var ret = new EventDataVM();
            var jsonStringData = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            jsonStringData = SeverHtmlDecode.Decode(this, jsonStringData);
            var items = jsonStringData.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var it in items)
            {
                if (it.Contains("id="))
                {
                    ret.id = it.Replace("id=", "").Trim();
                }
                if (it.Contains("title="))
                {
                    ret.title = SeverHtmlDecode.Decode(this,it.Replace("title=", "")).Replace("'", "").Replace("+", " ").Trim();
                }
                if (it.Contains("className"))
                {
                    ret.className = ret.className +":::"+ SeverHtmlDecode.Decode(this, it.Split(new string[] { "="},StringSplitOptions.RemoveEmptyEntries)[1]).Replace("'", "").Replace("+", " ").Trim();
                }
                if (it.Contains("description="))
                {
                    ret.desc = SeverHtmlDecode.Decode(this, it.Replace("description=", "")).Replace("'", "").Replace("+", " ").Trim();
                }
                if (it.Contains("start="))
                {
                    ret.start = SeverHtmlDecode.Decode(this, it.Replace("start=", "")).Replace("'", "").Replace("+", " ").Trim();
                }
                if (it.Contains("end="))
                {
                    ret.end = SeverHtmlDecode.Decode(this, it.Replace("end=", "")).Replace("'", "").Replace("+", " ").Trim();
                }
            }
            return ret;
        }



        [HttpPost]
        public JsonResult AddEvent()
        {
            var vm = ParseEventJason();
            var eventid = GetUniqKey();

            var res = new JsonResult();
            res.Data = new { success = true, id = eventid };
            return res;
        }

        public JsonResult UpdateEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        public JsonResult RemoveEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        public JsonResult MoveEvent()
        {
            var vm = ParseEventJason();

            var res = new JsonResult();
            res.Data = new { success = true };
            return res;
        }

        private void CreateAllDefaultCards(int cardcount, ECOBaseInfo baseinfo)
        {
            if (cardcount == 0)
                return;

            var currenttime = DateTime.Now;

            if (cardcount < 2)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOSignoff1, currenttime.ToString());
            }
            if (cardcount < 3)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());
            }
            if (cardcount < 4)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());
            }
            if (cardcount < 5)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());
            }
            if (cardcount < 6)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());
            }
            if (cardcount < 7)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());
            }
            if (cardcount < 8)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());
            }
        }

        // GET: MiniPIPs
        public ActionResult ViewAll()
        {
            //NebulaVM.CleanDB();

            //var baseinfo = new ECOBaseInfo();
            //baseinfo.ECOKey = NebulaVM.GetUniqKey();
            //baseinfo.ECONum = "97807";
            //baseinfo.PNDesc = "FCBG410QB1C10-FC";
            //baseinfo.Customer = "MRV";
            //baseinfo.PE = "Jessica Zheng";
            //baseinfo.CreateECO();

            //NebulaVM.CreateCard(baseinfo.ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOPending);

            //NebulaVM.RefreshSystem(this);

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser") && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {

            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ViewAll");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                if (string.IsNullOrEmpty(item.ECONum))
                    continue;

                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".","").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }



                var templist = NebulaVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, NebulaFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = NebulaVM.RetrieveECOCards(item);
                }

                if (string.Compare(item.MiniPIPStatus, NebulaMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = NebulaCardStatus.working;
                    }
                }

                bool forpereview = false;
                foreach (var card in templist)
                {
                    if (string.Compare(card.CardStatus, NebulaCardStatus.pending) == 0)
                    {
                        forpereview = true;
                    }
                }

                if (forpereview)
                {
                    vm.Add(templist);
                }
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { NebulaCardType.ECOPending, NebulaCardType.ECOSignoff1, NebulaCardType.CustomerApprovalHold
                                                        ,NebulaCardType.ECOComplete,NebulaCardType.SampleOrdering,NebulaCardType.SampleBuilding
                                                        ,NebulaCardType.SampleShipment,NebulaCardType.SampleCustomerApproval,NebulaCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = NebulaUserViewModels.RetrieveUserHistory(updater);

            GetNoticeInfo();

            return View(vm);
        }

        public ActionResult AllPendingWorkingMiniPIP()
        {

            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser") && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {

            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ViewAll");
                CookieUtility.SetCookie(this, ck);
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }

                var templist = NebulaVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, NebulaFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = NebulaVM.RetrieveECOCards(item);
                }

                if (string.Compare(item.FlowInfo, NebulaFlowInfo.Revise) == 0)
                {
                    if (templist.Count == 1 && string.Compare(item.ECOType, NebulaECOType.RVNS) == 0)
                    {
                        CreateRVNSCards(item.ECOKey);
                        templist = NebulaVM.RetrieveECOCards(item);
                    }
                    else if (templist.Count == 1 && string.Compare(item.ECOType, NebulaECOType.RVS) == 0)
                    {
                        CreateRVSCards(item.ECOKey);
                        templist = NebulaVM.RetrieveECOCards(item);
                    }
                }


                if (string.Compare(item.MiniPIPStatus, NebulaMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = NebulaCardStatus.working;
                    }
                }

                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { NebulaCardType.ECOPending, NebulaCardType.ECOSignoff1, NebulaCardType.CustomerApprovalHold
                                                        ,NebulaCardType.ECOComplete,NebulaCardType.SampleOrdering,NebulaCardType.SampleBuilding
                                                        ,NebulaCardType.SampleShipment,NebulaCardType.SampleCustomerApproval,NebulaCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = NebulaUserViewModels.RetrieveUserHistory(updater);

            GetNoticeInfo();

            return View("ViewAll", vm);
        }


        public ActionResult SummaryMiniPIP(string CardType)
        {
            var updater = GetAdminAuth();

            var fn = "MiniPIP-Summary-data-" + CardType.ToUpper().Replace(" ", "_") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }
            var realpath = imgdir + fn;
            var realurl = "/userfiles/docs/" + datestring + "/" + fn;
            logreportinfo(realpath, "Product requested,ECO NUM,customer,type,RSM,PE,Depart\r\n");

            var udlist = NebulaUserViewModels.RetrieveAllUserDepart();
            var uddict = new Dictionary<string, string>();
            foreach (var ud in udlist)
            {
                var name = ud.UserName.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper().Replace(".", "").Replace(" ", "");
                uddict.Add(name, ud.Depart);
            }

            var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();
            var vm = new List<List<NebulaVM>>();

            foreach (var item in baseinfos)
            {
                var depart = "";
                if (uddict.ContainsKey(item.PE.Replace(".", "").Replace(" ", "")))
                {
                    depart = uddict[item.PE.Replace(".", "").Replace(" ", "")];
                }

                var templist = NebulaVM.RetrieveECOCards(item);
                if (string.Compare(item.MiniPIPStatus, NebulaMiniPIPStatus.hold) == 0)
                {
                    foreach (var card in templist)
                    {
                        card.CardStatus = NebulaCardStatus.info;
                    }
                }

                foreach (var card in templist)
                {
                    if (string.Compare(card.CardStatus, NebulaCardStatus.pending) == 0
                        || string.Compare(card.CardStatus, NebulaCardStatus.working) == 0
                        || string.Compare(card.CardStatus, NebulaCardStatus.info) == 0)
                    {
                        if (string.Compare(card.CardType, NebulaCardType.ECOPending) == 0)
                        {
                            if (string.IsNullOrEmpty(item.ECONum))
                            {
                                if (string.Compare(CardType, NebulaCardType.ECOPending) == 0)
                                {
                                    vm.Add(templist);
                                    logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer+","+item.Complex + "," + item.RSM+ "," +item.PE + "," + depart + "\r\n");
                                    ViewBag.minipipsummaryurl = realurl;
                                }
                                break;
                            }
                        }

                        if (string.Compare(card.CardType, NebulaCardType.ECOSignoff1) == 0
                            || string.Compare(card.CardType, NebulaCardType.ECOSignoff2) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.ECOSignoff1) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }

                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.CustomerApprovalHold) == 0)
                        {
                            NebulaVM cardinfo = NebulaVM.RetrieveCustomerApproveHoldInfo(card.CardKey);
                            if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                            {
                                if (string.Compare(CardType, NebulaCardType.CustomerApprovalHold) == 0)
                                {
                                    vm.Add(templist);
                                    logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                    ViewBag.minipipsummaryurl = realurl;
                                }

                                break;
                            }
                        }

                        if (string.Compare(card.CardType, NebulaCardType.ECOComplete) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.ECOComplete) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.SampleOrdering) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.SampleOrdering) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.SampleBuilding) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.SampleBuilding) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.SampleShipment) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.SampleShipment) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.SampleCustomerApproval) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.SampleCustomerApproval) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        if (string.Compare(card.CardType, NebulaCardType.MiniPIPComplete) == 0)
                        {
                            if (string.Compare(CardType, NebulaCardType.MiniPIPComplete) == 0)
                            {
                                vm.Add(templist);
                                logreportinfo(realpath, item.PNDesc + "," + item.ECONum + "," + item.Customer + "," + item.Complex + "," + item.RSM + "," + item.PE + "," + depart + "\r\n");
                                ViewBag.minipipsummaryurl = realurl;
                            }
                            break;
                        }

                        break;
                    }//end if
                }//end foreach

            }//end foreach


            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { NebulaCardType.ECOPending, NebulaCardType.ECOSignoff1, NebulaCardType.CustomerApprovalHold
                                                        ,NebulaCardType.ECOComplete,NebulaCardType.SampleOrdering,NebulaCardType.SampleBuilding
                                                        ,NebulaCardType.SampleShipment,NebulaCardType.SampleCustomerApproval,NebulaCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = NebulaUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowPEWkingMiniPIP(string PE)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrievePEWorkingECOBaseInfo(PE);
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                var templist = NebulaVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, PE);

            var ecocardtypelistarray = new string[] { NebulaCardType.ECOPending, NebulaCardType.ECOSignoff1, NebulaCardType.CustomerApprovalHold
                                                        ,NebulaCardType.ECOComplete,NebulaCardType.SampleOrdering,NebulaCardType.SampleBuilding
                                                        ,NebulaCardType.SampleShipment,NebulaCardType.SampleCustomerApproval,NebulaCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = NebulaUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult ShowECOMiniPIP(string ECONum)
        {
            var updater = GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONum);
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                var templist = NebulaVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            var ecocardtypelistarray = new string[] { NebulaCardType.ECOPending, NebulaCardType.ECOSignoff1, NebulaCardType.CustomerApprovalHold
                                                        ,NebulaCardType.ECOComplete,NebulaCardType.SampleOrdering,NebulaCardType.SampleBuilding
                                                        ,NebulaCardType.SampleShipment,NebulaCardType.SampleCustomerApproval,NebulaCardType.MiniPIPComplete };
            asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(ecocardtypelistarray);
            ViewBag.ecocardtypelist = CreateSelectList(asilist, "");

            ViewBag.HistoryInfos = NebulaUserViewModels.RetrieveUserHistory(updater);
            GetNoticeInfo();

            return View("ViewAll", vm);
        }

        public ActionResult CompletedMiniPIP()
        {
            var updater = GetAdminAuth();

            var baseinfos = ECOBaseInfo.RetrieveAllCompletedECOBaseInfo();
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                var loginer = updater.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                var pe = item.PE.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace(".", "").Replace(" ", "").ToUpper();
                if (string.Compare(loginer, pe) != 0 && !(ViewBag.badmin || ViewBag.demo))
                    continue;

                if (ViewBag.demo)
                {
                    if (!ViewBag.demoecodict.ContainsKey(item.ECONum))
                    {
                        continue;
                    }
                }

                var templist = NebulaVM.RetrieveECOCards(item);
                if (templist.Count > 0 && string.Compare(item.FlowInfo, NebulaFlowInfo.Default) == 0)
                {
                    CreateAllDefaultCards(templist.Count, item);
                    templist = NebulaVM.RetrieveECOCards(item);
                }

                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, "");

            GetNoticeInfo();
            return View(vm);
        }

        public ActionResult ShowPECompletedMiniPIP(string PE)
        {
            GetAdminAuth();
            var baseinfos = ECOBaseInfo.RetrievePECompletedECOBaseInfo(PE);
            var vm = new List<List<NebulaVM>>();
            foreach (var item in baseinfos)
            {
                var templist = NebulaVM.RetrieveECOCards(item);
                vm.Add(templist);
            }

            var alluser = ECOBaseInfo.RetrieveAllPE();
            var asilist = new List<string>();
            asilist.Add("NONE");
            asilist.AddRange(alluser);
            ViewBag.FilterPEList = CreateSelectList(asilist, PE);

            return View("CompletedMiniPIP", vm);
        }



        private void logmaininfo(string info)
        {
            var Nebulafolder = "d:\\HeartBeat4Nebula";
            if (!Directory.Exists(Nebulafolder))
            {
                Directory.CreateDirectory(Nebulafolder);
            }

            var filename = Nebulafolder + "\\weblog" + DateTime.Now.ToString("yyyy-MM-dd");

            if (System.IO.File.Exists(filename))
            {
                var content = System.IO.File.ReadAllText(filename);
                content = content + info;
                System.IO.File.WriteAllText(filename, content);
            }
            else
            {
                System.IO.File.WriteAllText(filename, info);
            }
        }

        public ActionResult RefreshSys()
        {
            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\RefreshSys";
            if(Directory.Exists(imgdir))
                return RedirectToAction("ViewAll", "MiniPIP");

            try
            {
                Directory.CreateDirectory(imgdir);
            }catch (Exception ex) { }

            RefreshCardsInfo();
            //RefreshAgileInfo();

            return RedirectToAction("ViewAll", "MiniPIP");
        }

        private void RefreshCardsInfo()
        {
            try
            {
                NebulaDataCollector.SetForceECORefresh(this);
                NebulaDataCollector.RefreshECOList(this);

                logmaininfo(DateTime.Now.ToString() + "    ECO base info is refreshed!\r\n");

                var cardtypes = new string[] { NebulaCardType.ECOPending,NebulaCardType.ECOSignoff1
                                                , NebulaCardType.ECOSignoff2, NebulaCardType.SampleOrdering
                                                ,NebulaCardType.SampleBuilding,NebulaCardType.SampleShipment };
                var cardtypelist = new List<string>();
                cardtypelist.AddRange(cardtypes);

                var baseinfos = ECOBaseInfo.RetrieveAllWorkingECOBaseInfo();

                foreach (var cardtype in cardtypelist)
                {
                    var cardlist = new List<NebulaVM>();
                    foreach (var bs in baseinfos)
                    {
                        if (string.Compare(bs.MiniPIPStatus, NebulaMiniPIPStatus.hold) == 0)
                        {
                            continue;
                        }//for hold card,we will not update info

                        var ret = NebulaVM.RetrieveWorkingCard(bs, cardtype);
                        if (!string.IsNullOrEmpty(ret.CardKey))
                        {
                            cardlist.Add(ret);
                        }
                    }

                    foreach (var card in cardlist)
                    {
                        if (string.Compare(cardtype, NebulaCardType.ECOPending) == 0)
                        {
                            NebulaDataCollector.UpdateECOWeeklyUpdate(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO "+card.EBaseInfo.ECONum+"  ECOPending info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, NebulaCardType.ECOSignoff1) == 0)
                        {
                            NebulaDataCollector.RefreshQAFAI(card.EBaseInfo, card.CardKey, this);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  ECOSignoff1 info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, NebulaCardType.ECOSignoff2) == 0)
                        {
                            NebulaDataCollector.RefreshQAFAI(card.EBaseInfo, card.CardKey, this);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  ECOSignoff2 info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, NebulaCardType.SampleOrdering) == 0)
                        {
                            NebulaDataCollector.UpdateOrderInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleOrdering info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, NebulaCardType.SampleBuilding) == 0)
                        {
                            NebulaDataCollector.UpdateJOInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            NebulaDataCollector.Update1STJOCheckFromExcel(this, card.EBaseInfo, card.CardKey);
                            NebulaDataCollector.Update2NDJOCheckFromExcel(this, card.EBaseInfo, card.CardKey);
                            NebulaDataCollector.UpdateJOMainStoreFromExcel(this, card.EBaseInfo, card.CardKey);
                            NebulaDataCollector.RefreshTnuableQAFAI(this, card.EBaseInfo, card.CardKey);

                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleBuilding info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                        else if (string.Compare(cardtype, NebulaCardType.SampleShipment) == 0)
                        {
                            NebulaDataCollector.UpdateShipInfoFromExcel(this, card.EBaseInfo, card.CardKey);
                            logmaininfo(DateTime.Now.ToString() + " updated ECO " + card.EBaseInfo.ECONum + "  SampleShipment info\r\n");
                            NebulaVM.CardCanbeUpdate(card.CardKey);
                        }
                    }//end foreach

                    if (cardlist.Count > 0)
                    {
                        logmaininfo(DateTime.Now.ToString() + "    " + cardtype + " info is refreshed!\r\n");
                    }
                }//end foreach
            }catch (Exception ex) { }
        }

        private void RefreshAgileInfo()
        {
            var ecolist = ECOBaseInfo.RetrieveECOUnCompletedBaseInfo();
            var econumlist = new List<string>();
            foreach (var eco in ecolist)
            {
                econumlist.Add(eco.ECONum);
            }

            logmaininfo(DateTime.Now.ToString() + "    " + "start refreshing agile attachement name.....\r\n");
            NebulaDataCollector.DownloadAgile(econumlist, this, NebulaAGILEDOWNLOADTYPE.ATTACHNAME);
            logmaininfo(DateTime.Now.ToString() + "    " + "agile attachement name is refreshed.....\r\n");

            logmaininfo(DateTime.Now.ToString() + "    " + "start refreshing agile workflow.....\r\n");
            NebulaDataCollector.DownloadAgile(econumlist, this, NebulaAGILEDOWNLOADTYPE.WORKFLOW);
            logmaininfo(DateTime.Now.ToString() + "    " + "agile workflow is refreshed.....\r\n");
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

        private bool LoginSystem(Dictionary<string,string> ckdict, string ECOKey, string CardKey)
        {
            if (ckdict.ContainsKey("logonuser") 
                && !string.IsNullOrEmpty(ckdict["logonuser"]))
            {
                return true;
            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonredirectctrl", "MiniPIP");
                ck.Add("logonredirectact", "ECOPending");
                ck.Add("ECOKey", ECOKey);
                ck.Add("CardKey", CardKey);
                CookieUtility.SetCookie(this, ck);
                return false;
            }
        }

        private string GetAdminAuth()
        {
            ViewBag.badmin = false;
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("logonuser"))
            {
                ViewBag.badmin = NebulaUserViewModels.IsAdmin(ckdict["logonuser"].Split(new char[] { '|' })[0]);
                ViewBag.demo = NebulaUserViewModels.IsDemo(ckdict["logonuser"].Split(new char[] { '|' })[0]);

                var syscfgdict = CfgUtility.GetSysConfig(this);
                var demoecolist = syscfgdict["DEMOECONUM"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var demoecodict = new Dictionary<string, bool>();
                foreach (var demo in demoecolist)
                {
                    demoecodict.Add(demo.Trim(), true);
                }
                ViewBag.demoecodict = demoecodict;

                return ckdict["logonuser"].Split(new char[] { '|' })[0];
            }

            return string.Empty;
        }

        public ActionResult ECOPending(string ECOKey, string CardKey,string Refresh="No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                var ecoflows = new string[] { NebulaFlowInfo.Default, NebulaFlowInfo.Revise };
                var asilist = new List<string>();
                asilist.AddRange(ecoflows);
                ViewBag.FlowInfoList = CreateSelectList(asilist, baseinfos[0].FlowInfo);

                var pnimpls = new string[] { NebulaPNImplement.NA, NebulaPNImplement.Roll, NebulaPNImplement.CutOverImm, NebulaPNImplement.CutOverAft };
                asilist = new List<string>();
                asilist.AddRange(pnimpls);
                ViewBag.PNImplementList= CreateSelectList(asilist, baseinfos[0].PNImplement);

                var alluser = NebulaUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                foreach (var u in alluser)
                {
                    asilist.Add(u.Split(new char[] { '@' })[0].Replace(".", " ").ToUpper());
                }
                ViewBag.ActualPEList = CreateSelectList(asilist, baseinfos[0].PE.ToUpper());

                foreach (var card in cardlist)
                {
                    if (string.Compare(baseinfos[0].MiniPIPStatus, NebulaMiniPIPStatus.hold) == 0)
                    {
                        card.CardStatus = NebulaCardStatus.info;
                    }

                    if (string.Compare(card.CardType, NebulaCardType.ECOPending) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                NebulaVM cardinfo = NebulaVM.RetrieveECOPendingInfo(ViewBag.CurrentCard.CardKey);
                if (string.IsNullOrEmpty(cardinfo.CardKey))
                {
                    NebulaVM.UpdateECOPendingHoldInfo(ViewBag.CurrentCard.CardKey, NebulaYESNO.NO);
                }

                var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.ECOPending);
                if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        NebulaDataCollector.UpdateECOWeeklyUpdate(this, baseinfos[0], CardKey);
                        NebulaDataCollector.RefreshECOPendingAttachInfo(this, baseinfos[0].ECOKey);
                    }
                }

                ViewBag.CurrentCard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.ECOPending)[0];

                cardinfo = NebulaVM.RetrieveECOPendingInfo(CardKey);
                ViewBag.CurrentCard.MiniPIPHold = cardinfo.MiniPIPHold;

                var historys = NebulaVM.RetrievePendingHistoryInfo(CardKey);
                ViewBag.CurrentCard.PendingHistoryTable = historys;

                var pipholds = new string[] { NebulaYESNO.NO,NebulaYESNO.YES};
                asilist = new List<string>();
                asilist.AddRange(pipholds);
                ViewBag.MiniPIPHoldList = CreateSelectList(asilist, cardinfo.MiniPIPHold);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.ECOPending;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                if (string.IsNullOrEmpty(baseinfos[0].ECONum))
                {
                    ViewBag.PendingDays = "0";
                    if (!string.IsNullOrEmpty(baseinfos[0].FinalRevison))
                    {
                        ViewBag.PendingDays = (DateTime.Now - DateTime.Parse(baseinfos[0].FinalRevison)).Days.ToString();
                    }
                    else
                    {
                        ViewBag.PendingDays = (DateTime.Now - DateTime.Parse(baseinfos[0].InitRevison)).Days.ToString();
                    }
                    
                    NebulaVM.UpdateECOPendingPendingDays(ViewBag.CurrentCard.CardKey, ViewBag.PendingDays);
                }
                else
                {
                    if (string.IsNullOrEmpty(cardinfo.ECOPendingDays))
                    {
                        ViewBag.PendingDays = "0";
                        NebulaVM.UpdateECOPendingPendingDays(ViewBag.CurrentCard.CardKey, "0");
                    }
                    else
                    {
                        ViewBag.PendingDays = cardinfo.ECOPendingDays;
                    }
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");
        }

        private static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }


        public void SetNoticeInfo(string noticinfo)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("NebulaNoticeInfo", noticinfo);
            CookieUtility.SetCookie(this, dict);
        }

        public void GetNoticeInfo()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (ckdict.ContainsKey("NebulaNoticeInfo"))
            {
                if (!string.IsNullOrEmpty(ckdict["NebulaNoticeInfo"]))
                {
                    ViewBag.NebulaNoticeInfo = ckdict["NebulaNoticeInfo"];
                    SetNoticeInfo(string.Empty);
                }
                else
                {
                    ViewBag.NebulaNoticeInfo = null;
                }
            }
            else
            {
                ViewBag.NebulaNoticeInfo = null;
            }
        }

        [HttpPost, ActionName("ECOPending")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOPendingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];
            
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                if (IsDigitsOnly(Request.Form["OrderInfo"].Trim()))
                {
                    baseinfos[0].FirstArticleNeed = Request.Form["OrderInfo"].Trim();
                }
                else
                {
                    baseinfos[0].FirstArticleNeed = "N/A";
                }

                baseinfos[0].FlowInfo = Request.Form["FlowInfoList"].ToString();

                if (IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, NebulaFlowInfo.Default, true) == 0)
                {
                    baseinfos[0].ECOType = NebulaECOType.DVS;
                }
                else if (IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, NebulaFlowInfo.Revise, true) == 0)
                {
                    baseinfos[0].ECOType = NebulaECOType.RVS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, NebulaFlowInfo.Default, true) == 0)
                {
                    baseinfos[0].ECOType = NebulaECOType.DVNS;
                }
                else if (!IsDigitsOnly(baseinfos[0].FirstArticleNeed)
                    && string.Compare(baseinfos[0].FlowInfo, NebulaFlowInfo.Revise, true) == 0)
                {
                    baseinfos[0].ECOType = NebulaECOType.RVNS;
                }

                baseinfos[0].MCOIssued = Request.Form["MCOIssued"];
                baseinfos[0].PNImplement = Request.Form["PNImplementList"].ToString();
                //baseinfos[0].FACustomerApproval = Request.Form["FACustomerApproval"];
                var ecohold = Request.Form["MiniPIPHoldList"].ToString();
                if (string.Compare(ecohold, NebulaYESNO.YES) == 0)
                {
                    baseinfos[0].MiniPIPStatus = NebulaMiniPIPStatus.hold;
                    if (DateTime.Parse(baseinfos[0].ECOHoldStartDate).ToString("yyyy-MM-dd").Contains("1982-05-06")
                        && DateTime.Parse(baseinfos[0].ECOHoldEndDate).ToString("yyyy-MM-dd").Contains("1982-05-06"))
                    {
                        baseinfos[0].ECOHoldStartDate = DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00";
                    }
                }
                else
                {
                    baseinfos[0].MiniPIPStatus = NebulaMiniPIPStatus.working;
                    if (!DateTime.Parse(baseinfos[0].ECOHoldStartDate).ToString("yyyy-MM-dd").Contains("1982-05-06")
                        && DateTime.Parse(baseinfos[0].ECOHoldEndDate).ToString("yyyy-MM-dd").Contains("1982-05-06"))
                    {
                        baseinfos[0].ECOHoldEndDate = DateTime.Now.ToString("yyyy-MM-dd") + " 07:30:00";
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["ActualPEList"]))
                {
                    if (string.Compare(Request.Form["ActualPEList"].ToString(), "NONE") != 0)
                    {
                        baseinfos[0].ActualPE = Request.Form["ActualPEList"].ToString();
                    }
                }

                baseinfos[0].UpdateECO();
                NebulaVM.UpdateECOPendingHoldInfo(CardKey, ecohold);
                
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }


                if (Request.Form["forcecard"] == null)
                {
                    if (string.IsNullOrEmpty(baseinfos[0].ECONum))
                    {
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        SetNoticeInfo("ECO Number is not ready");

                        return RedirectToAction(NebulaCardType.ECOPending, "MiniPIP", dict);
                    }

                    if (string.Compare(ecohold, NebulaYESNO.YES) == 0)
                    {
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        SetNoticeInfo("ECO Status is on hold");

                        return RedirectToAction(NebulaCardType.ECOPending, "MiniPIP", dict);
                    }
                }


                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();

                if (string.Compare(baseinfos[0].ECOType, NebulaECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, NebulaECOType.DVNS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOSignoff1, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(NebulaCardType.ECOSignoff1, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOSignoff2, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.CustomerApprovalHold, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(NebulaCardType.ECOSignoff2, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVNS) == 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(2000);
                    var currenttime = DateTime.Now;

                    currenttime = currenttime.AddMinutes(1);

                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOSignoff2, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.CustomerApprovalHold, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());

                    currenttime = currenttime.AddMinutes(1);
                    NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());

                    return RedirectToAction(NebulaCardType.ECOSignoff2, "MiniPIP", dict);
                }
                //else
                //{
                //    new System.Threading.ManualResetEvent(false).WaitOne(1000);
                //    var currenttime = DateTime.Now;
                //    currenttime = currenttime.AddMinutes(1);

                //    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOSignoff1,currenttime.ToString());
                //    var dict = new RouteValueDictionary();
                //    dict.Add("ECOKey", ECOKey);
                //    dict.Add("CardKey", realcardkey);
                //    return RedirectToAction(NebulaCardType.ECOSignoff1, "MiniPIP", dict);
                //}
                var dict1 = new RouteValueDictionary();
                dict1.Add("ECOKey", ECOKey);
                dict1.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.ECOPending, "MiniPIP", dict1);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.ECOPending, "MiniPIP", dict);
            }
        }

        public ActionResult ECOSignoff1(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                    var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.ECOSignoff1);
                    if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                        || string.Compare(Refresh, "YES", true) == 0)
                    {
                        if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                        {
                            NebulaDataCollector.RefreshQAFAI(baseinfos[0], CardKey, this);
                        }
                    }//if card is not finished,we refresh qa folder to get files


                bool eepromattachexist = false;
                bool labelattachexist = false;

                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.ECOSignoff1) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        foreach (var attach in card.AttachList)
                        {
                            if (attach.ToUpper().Contains("EEPROM"))
                            {
                                eepromattachexist = true;
                            }
                            if (attach.ToUpper().Contains("_FAI_"))
                            {
                                labelattachexist = true;
                            }
                        }
                        break;
                    }
                }

                NebulaVM cardinfo = NebulaVM.RetrieveSignoffInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.QAEEPROMCheck = cardinfo.QAEEPROMCheck;
                ViewBag.CurrentCard.QALabelCheck = cardinfo.QALabelCheck;
                ViewBag.CurrentCard.PeerReviewEngineer = cardinfo.PeerReviewEngineer;
                ViewBag.CurrentCard.PeerReview = cardinfo.PeerReview;
                ViewBag.CurrentCard.ECOAttachmentCheck = cardinfo.ECOAttachmentCheck;
                ViewBag.CurrentCard.ECOQRFile = cardinfo.ECOQRFile;
                ViewBag.CurrentCard.EEPROMPeerReview = cardinfo.EEPROMPeerReview;
                ViewBag.CurrentCard.ECOTraceview = cardinfo.ECOTraceview;
                ViewBag.CurrentCard.SpecCompresuite = cardinfo.SpecCompresuite;
                ViewBag.CurrentCard.ECOTRApprover = cardinfo.ECOTRApprover;
                ViewBag.CurrentCard.ECOMDApprover = cardinfo.ECOMDApprover;
                ViewBag.CurrentCard.MiniPVTCheck = cardinfo.MiniPVTCheck;
                ViewBag.CurrentCard.AgileCodeFile = cardinfo.AgileCodeFile;
                ViewBag.CurrentCard.AgileSpecFile = cardinfo.AgileSpecFile;
                ViewBag.CurrentCard.AgileTestFile = cardinfo.AgileTestFile;
                ViewBag.CurrentCard.FACategory = cardinfo.FACategory;
                ViewBag.CurrentCard.RSMSendDate = cardinfo.RSMSendDate;
                ViewBag.CurrentCard.RSMApproveDate = cardinfo.RSMApproveDate;
                

                if (!string.IsNullOrEmpty(cardinfo.RSMSendDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMSendDate = DateTime.Parse(cardinfo.RSMSendDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMApproveDate = DateTime.Parse(cardinfo.RSMApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { NebulaYESNO.NO, NebulaYESNO.YES };

                var asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QAEEPROMCheck) && eepromattachexist)
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist,NebulaYESNO.YES);
                }
                else
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);
                }
                
              
                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QALabelCheck) && labelattachexist)
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, NebulaYESNO.YES);
                }
                else
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);
                }

                var alluser = NebulaUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);


                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] {"N/A",NebulaFACategory.EEPROMFA, NebulaFACategory.LABELFA, NebulaFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.ECOSignoff1;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("ECOSignoff1")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff1Post()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                
                var cardinfo = NebulaVM.RetrieveSignoffInfo(CardKey);

                cardinfo.QAEEPROMCheck = Request.Form["QAEEPROMCheckList"].ToString();
                cardinfo.QALabelCheck = Request.Form["QALabelCheckList"].ToString();
                cardinfo.PeerReviewEngineer = Request.Form["PeerReviewEngineerList"].ToString();
                cardinfo.PeerReview = Request.Form["PeerReviewList"].ToString();
                cardinfo.ECOAttachmentCheck = Request.Form["ECOAttachmentCheckList"].ToString();
                cardinfo.MiniPVTCheck = Request.Form["MiniPVTCheckList"].ToString();
                cardinfo.FACategory = Request.Form["FACategoryList"].ToString();

                cardinfo.ECOTRApprover = Request.Form["ECOTRApprover"];
                cardinfo.ECOMDApprover = Request.Form["ECOMDApprover"];

                cardinfo.RSMSendDate = Request.Form["RSMSendDate"];
                cardinfo.RSMApproveDate = Request.Form["RSMApproveDate"];

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    baseinfos[0].FACustomerApproval = cardinfo.RSMApproveDate;
                    baseinfos[0].UpdateECO();
                }
                
                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.QAEEPROMCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA EEPROM is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.QALabelCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA Label is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.PeerReview, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Peer Review is not finish");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.ECOAttachmentCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO Attachement is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.MiniPVTCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Mini PVT is not checked");
                        allchecked = false;
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.ECOSignoff1, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOComplete,currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(NebulaCardType.ECOComplete, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.ECOSignoff1, "MiniPIP", dict);
            }
}


        public ActionResult ECOComplete(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.ECOComplete) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                NebulaVM cardinfo = NebulaVM.RetrieveECOCompleteInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.ECOCompleted = cardinfo.ECOCompleted;
                ViewBag.CurrentCard.ECOCompleteDate = cardinfo.ECOCompleteDate;


                if (!string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCompleteDate = DateTime.Parse(cardinfo.ECOCompleteDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { NebulaYESNO.NO, NebulaYESNO.YES };
                var asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.ECOCompletedList = CreateSelectList(asilist, cardinfo.ECOCompleted);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.ECOComplete;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        private static string ConvertToDate(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Trim()))
                {
                    return string.Empty;
                }

                var date = DateTime.Parse(Convert.ToString(obj));
                return date.ToString("yyyy-MM-dd");
            }
            catch (Exception ex) { return string.Empty; }
        }

        private static string ConvertUSLocalToDate(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Trim()))
                {
                    return string.Empty;
                }
                CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                var date = DateTime.ParseExact(obj.Trim().Replace("CST", "-6"), "ddd MMM dd HH:mm:ss z yyyy", culture);
                return date.ToString("yyyy-MM-dd hh:mm:ss");
            }
            catch (Exception ex) {
                return string.Empty;
            }
        }


        [HttpPost, ActionName("ECOComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOCompletePost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                NebulaVM cardinfo = NebulaVM.RetrieveECOCompleteInfo(CardKey);
                cardinfo.ECOCompleted = Request.Form["ECOCompletedList"].ToString();
                cardinfo.ECOCompleteDate = ConvertToDate(Request.Form["ECOCompleteDate"]);
                cardinfo.UpdateECOCompleteInfo(CardKey);

                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.ECOCompleted, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO should be completed");
                        allchecked = false;
                    }
                    else if (string.IsNullOrEmpty(cardinfo.ECOCompleteDate))
                    {
                        SetNoticeInfo("ECO Complete Date is needed");
                        allchecked = false;
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.ECOComplete, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var newcardkey = NebulaVM.GetUniqKey();
                if (string.Compare(baseinfos[0].ECOType, NebulaECOType.DVS) == 0
                    || string.Compare(baseinfos[0].ECOType, NebulaECOType.RVNS) == 0
                    || string.Compare(baseinfos[0].ECOType, NebulaECOType.DVNS) == 0)
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVS) == 0)
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.MiniPIPComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.MiniPIPComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.ECOComplete, "MiniPIP", dict);
            }
        }


        public ActionResult ECOSignoff2(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.ECOSignoff2);
                if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        NebulaDataCollector.RefreshQAFAI(baseinfos[0], CardKey, this);
                    }
                }//if card is not finished,we refresh qa folder to get files
            
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                bool eepromattachexist = false;
                bool labelattachexist = false;

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.ECOSignoff2) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        foreach (var attach in card.AttachList)
                        {
                            if (attach.ToUpper().Contains("EEPROM"))
                            {
                                eepromattachexist = true;
                            }
                            if (attach.ToUpper().Contains("_FAI_"))
                            {
                                labelattachexist = true;
                            }
                        }
                        break;
                    }
                }

                NebulaVM cardinfo = NebulaVM.RetrieveSignoffInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.QAEEPROMCheck = cardinfo.QAEEPROMCheck;
                ViewBag.CurrentCard.QALabelCheck = cardinfo.QALabelCheck;
                ViewBag.CurrentCard.PeerReviewEngineer = cardinfo.PeerReviewEngineer;
                ViewBag.CurrentCard.PeerReview = cardinfo.PeerReview;
                ViewBag.CurrentCard.ECOAttachmentCheck = cardinfo.ECOAttachmentCheck;
                ViewBag.CurrentCard.ECOQRFile = cardinfo.ECOQRFile;
                ViewBag.CurrentCard.EEPROMPeerReview = cardinfo.EEPROMPeerReview;
                ViewBag.CurrentCard.ECOTraceview = cardinfo.ECOTraceview;
                ViewBag.CurrentCard.SpecCompresuite = cardinfo.SpecCompresuite;
                ViewBag.CurrentCard.ECOTRApprover = cardinfo.ECOTRApprover;
                ViewBag.CurrentCard.ECOMDApprover = cardinfo.ECOMDApprover;
                ViewBag.CurrentCard.MiniPVTCheck = cardinfo.MiniPVTCheck;
                ViewBag.CurrentCard.AgileCodeFile = cardinfo.AgileCodeFile;
                ViewBag.CurrentCard.AgileSpecFile = cardinfo.AgileSpecFile;
                ViewBag.CurrentCard.AgileTestFile = cardinfo.AgileTestFile;
                ViewBag.CurrentCard.FACategory = cardinfo.FACategory;
                ViewBag.CurrentCard.RSMSendDate = cardinfo.RSMSendDate;
                ViewBag.CurrentCard.RSMApproveDate = cardinfo.RSMApproveDate;
                ViewBag.CurrentCard.ECOCustomerHoldDate = cardinfo.ECOCustomerHoldDate;

                if (!string.IsNullOrEmpty(cardinfo.RSMSendDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMSendDate = DateTime.Parse(cardinfo.RSMSendDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.RSMApproveDate = DateTime.Parse(cardinfo.RSMApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerHoldDate = DateTime.Parse(cardinfo.ECOCustomerHoldDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                var yesno = new string[] { NebulaYESNO.NO, NebulaYESNO.YES };

                var asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QAEEPROMCheck) && eepromattachexist)
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, NebulaYESNO.YES);
                }
                else
                {
                    ViewBag.QAEEPROMCheckList = CreateSelectList(asilist, cardinfo.QAEEPROMCheck);
                }

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);

                if (string.IsNullOrEmpty(cardinfo.QALabelCheck) && labelattachexist)
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, NebulaYESNO.YES);
                }
                else
                {
                    ViewBag.QALabelCheckList = CreateSelectList(asilist, cardinfo.QALabelCheck);
                }

                var alluser = NebulaUserViewModels.RetrieveAllUser();
                asilist = new List<string>();
                asilist.Add("NONE");
                asilist.AddRange(alluser);
                ViewBag.PeerReviewEngineerList = CreateSelectList(asilist, cardinfo.PeerReviewEngineer);

                asilist = new List<string>();
                asilist.AddRange(yesno);
                ViewBag.PeerReviewList = CreateSelectList(asilist, cardinfo.PeerReview);

                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.ECOAttachmentCheckList = CreateSelectList(asilist, cardinfo.ECOAttachmentCheck);


                asilist = new List<string>();
                asilist.Add("N/A");
                asilist.AddRange(yesno);
                ViewBag.MiniPVTCheckList = CreateSelectList(asilist, cardinfo.MiniPVTCheck);

                var facats = new string[] { "N/A",NebulaFACategory.EEPROMFA, NebulaFACategory.LABELFA, NebulaFACategory.LABELEEPROMFA };
                asilist = new List<string>();
                asilist.AddRange(facats);
                ViewBag.FACategoryList = CreateSelectList(asilist, cardinfo.FACategory);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.ECOSignoff2;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }
                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("ECOSignoff2")]
        [ValidateAntiForgeryToken]
        public ActionResult ECOSignoff2Post()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {

                var cardinfo = NebulaVM.RetrieveSignoffInfo(CardKey);

                cardinfo.QAEEPROMCheck = Request.Form["QAEEPROMCheckList"].ToString();
                cardinfo.QALabelCheck = Request.Form["QALabelCheckList"].ToString();
                cardinfo.PeerReviewEngineer = Request.Form["PeerReviewEngineerList"].ToString();
                cardinfo.PeerReview = Request.Form["PeerReviewList"].ToString();
                cardinfo.ECOAttachmentCheck = Request.Form["ECOAttachmentCheckList"].ToString();
                cardinfo.MiniPVTCheck = Request.Form["MiniPVTCheckList"].ToString();
                cardinfo.FACategory = Request.Form["FACategoryList"].ToString();

                cardinfo.ECOTRApprover = Request.Form["ECOTRApprover"];
                cardinfo.ECOMDApprover = Request.Form["ECOMDApprover"];

                cardinfo.RSMSendDate = Request.Form["RSMSendDate"];
                cardinfo.RSMApproveDate = Request.Form["RSMApproveDate"];

                if (!string.IsNullOrEmpty(cardinfo.RSMApproveDate))
                {
                    baseinfos[0].FACustomerApproval = cardinfo.RSMApproveDate;
                    baseinfos[0].UpdateECO();
                }

                cardinfo.ECOCustomerHoldDate = Request.Form["ECOCustomerHoldDate"];
                
                StoreAttachAndComment(CardKey, updater, cardinfo);

                cardinfo.UpdateSignoffInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;
                    if (string.Compare(cardinfo.QAEEPROMCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA EEPROM is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.QALabelCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("QA Label is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.PeerReview, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Peer Review is not finish");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.ECOAttachmentCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("ECO Attachement is not checked");
                        allchecked = false;
                    }
                    else if (string.Compare(cardinfo.MiniPVTCheck, NebulaYESNO.NO) == 0)
                    {
                        SetNoticeInfo("Mini PVT is not checked");
                        allchecked = false;
                    }
                    //else if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldDate))
                    //{
                    //    SetNoticeInfo("ECO Customer Hold Date need to be inputed");
                    //    allchecked = false;
                    //}

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.ECOSignoff2, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.CustomerApprovalHold, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(NebulaCardType.CustomerApprovalHold, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.ECOSignoff2, "MiniPIP", dict);
            }
        }


        public ActionResult CustomerApprovalHold(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.CustomerApprovalHold) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                NebulaVM cardinfo = NebulaVM.RetrieveCustomerApproveHoldInfo(ViewBag.CurrentCard.CardKey);
                ViewBag.CurrentCard.ECOCustomerApproveDate = cardinfo.ECOCustomerApproveDate;
                ViewBag.CurrentCard.ECOCustomerHoldStartDate = cardinfo.ECOCustomerHoldStartDate;
                ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerApproveDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerApproveDate = DateTime.Parse(cardinfo.ECOCustomerApproveDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }

                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                {
                    try
                    {
                        ViewBag.CurrentCard.ECOCustomerHoldStartDate = DateTime.Parse(cardinfo.ECOCustomerHoldStartDate).ToString("yyyy-MM-dd");
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    //try to get customeholddate from signoff2 card to retrieve previous stored data
                    var signoffcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.ECOSignoff2);
                    if (signoffcard.Count > 0)
                    {
                        var signoffinfo = NebulaVM.RetrieveSignoffInfo(signoffcard[0].CardKey);
                        if (!string.IsNullOrEmpty(signoffinfo.ECOCustomerHoldDate))
                        {
                            try
                            {
                                ViewBag.CurrentCard.ECOCustomerHoldStartDate = DateTime.Parse(signoffinfo.ECOCustomerHoldDate).ToString("yyyy-MM-dd");
                                cardinfo.ECOCustomerHoldStartDate = signoffinfo.ECOCustomerHoldDate;
                                cardinfo.UpdateCustomerApproveHoldStartDate(ViewBag.CurrentCard.CardKey);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }

                if (string.Compare(ViewBag.CurrentCard.CardStatus,NebulaCardStatus.done,true) != 0)
                {
                    if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                    {
                        //only hold start date exist, we can compute the hold aging
                        cardinfo.ECOCustomerHoldAging = (DateTime.Now - DateTime.Parse(cardinfo.ECOCustomerHoldStartDate)).Days.ToString();
                        ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;
                        cardinfo.UpdateCustomerApproveHoldAging(ViewBag.CurrentCard.CardKey);
                    }
                }
                else
                {
                    //if card is pass but hold aging is empty, we set it to 0
                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldAging))
                    {
                        cardinfo.ECOCustomerHoldAging = "0";
                        ViewBag.CurrentCard.ECOCustomerHoldAging = cardinfo.ECOCustomerHoldAging;
                        cardinfo.UpdateCustomerApproveHoldAging(ViewBag.CurrentCard.CardKey);
                    }
                }

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.CustomerApprovalHold;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("CustomerApprovalHold")]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerApprovalHoldPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {

                StoreAttachAndComment(CardKey, updater);
                NebulaVM cardinfo = NebulaVM.RetrieveCustomerApproveHoldInfo(CardKey);

                cardinfo.ECOCustomerHoldStartDate = Request.Form["ECOCustomerHoldStartDate"];
                if (!string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                {
                    cardinfo.UpdateCustomerApproveHoldStartDate(CardKey);
                }

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                
                //cardinfo.ECOCustomerApproveDate = Request.Form["ECOCustomerApproveDate"];
                //cardinfo.UpdateCustomerApproveHoldInfo(CardKey);
                if (Request.Form["forcecard"] == null)
                {
                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerApproveDate) && string.IsNullOrEmpty(baseinfos[0].FACustomerApproval))
                    {
                        SetNoticeInfo("ECO Sample Approve Date or FAI Approve Date, At least one of them is inputed");
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.CustomerApprovalHold, "MiniPIP", dict);
                    }

                    if (string.IsNullOrEmpty(cardinfo.ECOCustomerHoldStartDate))
                    {
                        SetNoticeInfo("ECOCustomerHoldStartDate need to be inputed");
                        var dict = new RouteValueDictionary();
                        dict.Add("ECOKey", ECOKey);
                        dict.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.CustomerApprovalHold, "MiniPIP", dict);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVS) == 0)
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleOrdering,currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict);
                }
                else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVNS) == 0)
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleOrdering, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.CustomerApprovalHold, "MiniPIP", dict);
            }
        }


        public ActionResult SampleOrdering(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.SampleOrdering);
                if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        NebulaDataCollector.UpdateOrderInfoFromExcel(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.SampleOrdering) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                var orderinfo = NebulaVM.RetrieveOrderInfo(CardKey);
                ViewBag.CurrentCard.OrderTable = orderinfo;

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.SampleOrdering;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleOrdering")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleOrderingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var orderinfo = NebulaVM.RetrieveOrderInfo(CardKey);
                    if (orderinfo.Count == 0)
                    {
                        SetNoticeInfo("No order information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var newcardkey = NebulaVM.GetUniqKey();
                var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleBuilding, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(NebulaCardType.SampleBuilding, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.SampleOrdering, "MiniPIP", dict);
            }

        }


        public ActionResult SampleBuilding(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.SampleBuilding);
                if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        NebulaDataCollector.UpdateJOInfoFromExcel(this, baseinfos[0], CardKey);
                        NebulaDataCollector.Update1STJOCheckFromExcel(this, baseinfos[0], CardKey);
                        NebulaDataCollector.Update2NDJOCheckFromExcel(this, baseinfos[0], CardKey);
                        NebulaDataCollector.UpdateJOMainStoreFromExcel(this, baseinfos[0], CardKey);
                        NebulaDataCollector.RefreshTnuableQAFAI(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.SampleBuilding) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.CurrentCard.JoTable = NebulaVM.RetrieveJOInfo(CardKey);
                ViewBag.CurrentCard.Jo1stCheckTable = NebulaVM.RetrieveJOCheck(CardKey, NebulaJOCHECKTYPE.ENGTYPE);
                ViewBag.CurrentCard.Jo2ndCheckTable = NebulaVM.RetrieveJOCheck(CardKey, NebulaJOCHECKTYPE.QATYPE);
                ViewBag.CurrentCard.JOStoreStautsTable = NebulaVM.RetrieveJOMainStore(CardKey);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.SampleBuilding;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }
                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleBuilding")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleBuildingPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var JoTable = NebulaVM.RetrieveJOInfo(CardKey);
                    var FirstCheckTable = NebulaVM.RetrieveJOCheck(CardKey, NebulaJOCHECKTYPE.ENGTYPE);
                    var SecondCheckTable = NebulaVM.RetrieveJOCheck(CardKey, NebulaJOCHECKTYPE.QATYPE);

                    if (JoTable.Count == 0)
                    {
                        SetNoticeInfo("No JO information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                    else if (SecondCheckTable.Count == 0)
                    {
                        SetNoticeInfo("No EEPROM second check information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                    else if (FirstCheckTable.Count == 0)
                    {
                        SetNoticeInfo("No Engineering check information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleBuilding, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);
                var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleShipment, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(NebulaCardType.SampleShipment, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.SampleBuilding, "MiniPIP", dict);
            }
        }


        public ActionResult SampleShipment(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var currentcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.SampleShipment);
                if (string.Compare(currentcard[0].CardStatus, NebulaCardStatus.done, true) != 0
                    || string.Compare(Refresh, "YES", true) == 0)
                {
                    if (NebulaVM.CardCanbeUpdate(CardKey) || string.Compare(Refresh, "YES", true) == 0)
                    {
                        NebulaDataCollector.UpdateShipInfoFromExcel(this, baseinfos[0], CardKey);
                    }
                }

                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.SampleShipment) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.CurrentCard.ShipTable = NebulaVM.RetrieveShipInfo(CardKey);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.SampleShipment;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, currentcard[0].CardType, currentcard[0].CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleShipment")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleShipmentPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var shipinfo = NebulaVM.RetrieveShipInfo(CardKey);
                    if (shipinfo.Count == 0)
                    {
                        SetNoticeInfo("No Shipment information is found");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleShipment, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.SampleCustomerApproval, currenttime.ToString());

                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", realcardkey);
                return RedirectToAction(NebulaCardType.SampleCustomerApproval, "MiniPIP", dict);
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.SampleShipment, "MiniPIP", dict);
            }
        }

        public ActionResult SampleCustomerApproval(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.SampleCustomerApproval) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                var cardinfo = NebulaVM.RetrieveSampleCustomerApproveInfo(CardKey);
                ViewBag.CurrentCard.SampleCustomerApproveDate = cardinfo.SampleCustomerApproveDate;

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.SampleCustomerApproval;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");

        }

        [HttpPost, ActionName("SampleCustomerApproval")]
        [ValidateAntiForgeryToken]
        public ActionResult SampleCustomerApprovalPost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                var tempinfo = new NebulaVM();
                tempinfo.SampleCustomerApproveDate = Request.Form["SampleCustomerApproveDate"];
                tempinfo.UpdateSampleCustomerApproveInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }


                if (string.IsNullOrEmpty(tempinfo.SampleCustomerApproveDate))
                {
                    if (Request.Form["forcecard"] == null)
                    {
                        SetNoticeInfo("Sample Customer Approve Date is not inputed");
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.SampleCustomerApproval, "MiniPIP", dict1);
                    }
                }
                else
                {
                    var customerholdcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.CustomerApprovalHold);
                    if (customerholdcard.Count > 0)
                    {
                        customerholdcard[0].ECOCustomerApproveDate = tempinfo.SampleCustomerApproveDate;
                        customerholdcard[0].UpdateCustomerApproveHoldInfo(customerholdcard[0].CardKey);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                var newcardkey = NebulaVM.GetUniqKey();
                new System.Threading.ManualResetEvent(false).WaitOne(2000);
                var currenttime = DateTime.Now;
                currenttime = currenttime.AddMinutes(1);

                if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVS) == 0)
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.ECOComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.ECOComplete, "MiniPIP", dict);
                }
                else
                {
                    var realcardkey = NebulaVM.CreateCard(ECOKey, newcardkey, NebulaCardType.MiniPIPComplete, currenttime.ToString());
                    var dict = new RouteValueDictionary();
                    dict.Add("ECOKey", ECOKey);
                    dict.Add("CardKey", realcardkey);
                    return RedirectToAction(NebulaCardType.MiniPIPComplete, "MiniPIP", dict);
                }
            }
            else
            {
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", CardKey);
                return RedirectToAction(NebulaCardType.SampleCustomerApproval, "MiniPIP", dict);
            }
        }


        public ActionResult MiniPIPComplete(string ECOKey, string CardKey, string Refresh = "No")
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            if (!LoginSystem(ckdict, ECOKey, CardKey))
            {
                return RedirectToAction("LoginUser", "NebulaUser");
            }

            var updater = GetAdminAuth();

            if (string.IsNullOrEmpty(ECOKey))
                ECOKey = ckdict["ECOKey"];
            if (string.IsNullOrEmpty(CardKey))
                CardKey = ckdict["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                var vm = new List<List<NebulaVM>>();
                var cardlist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                vm.Add(cardlist);

                foreach (var card in cardlist)
                {
                    if (string.Compare(card.CardType, NebulaCardType.MiniPIPComplete) == 0)
                    {
                        ViewBag.CurrentCard = card;
                        break;
                    }
                }

                ViewBag.MCOIssued = baseinfos[0].MCOIssued;
                ViewBag.FACustomerApproval = baseinfos[0].FACustomerApproval;

                var approvecard = NebulaVM.RetrieveSpecialCard(baseinfos[0], NebulaCardType.SampleCustomerApproval);
                var approvecardinfo = NebulaVM.RetrieveSampleCustomerApproveInfo(approvecard[0].CardKey);
                ViewBag.SampleCustomerApproveDate = approvecardinfo.SampleCustomerApproveDate;

                var cardinfo = NebulaVM.RetrieveMinipipCompleteInfo(CardKey);

                var lifecycle = new string[] {"NONE", NebulaLifeCycle.FirstArticl, NebulaLifeCycle.Prototype,NebulaLifeCycle.PreProduct, NebulaLifeCycle.Pilot, NebulaLifeCycle.Production};
                var asilist = new List<string>();
                asilist.AddRange(lifecycle);
                ViewBag.ECOPartLifeCycleList = CreateSelectList(asilist,cardinfo.ECOPartLifeCycle);

                asilist = new List<string>();
                asilist.AddRange(lifecycle);
                ViewBag.GenericPartLifeCycleList = CreateSelectList(asilist,cardinfo.GenericPartLifeCycle);

                ViewBag.ECOKey = ECOKey;
                ViewBag.CardKey = CardKey;
                ViewBag.CardDetailPage = NebulaCardType.MiniPIPComplete;

                if (!string.IsNullOrEmpty(updater))
                {
                    NebulaUserViewModels.UpdateUserHistory(updater, baseinfos[0].ECONum, ViewBag.CurrentCard.CardType, ViewBag.CurrentCard.CardKey);
                }

                GetNoticeInfo();
                return View("CurrentECO", vm);
            }

            return RedirectToAction("ViewAll", "MiniPIP");
        }


        [HttpPost, ActionName("MiniPIPComplete")]
        [ValidateAntiForgeryToken]
        public ActionResult MiniPIPCompletePost()
        {
            var ckdict = CookieUtility.UnpackCookie(this);
            var updater = ckdict["logonuser"].Split(new char[] { '|' })[0];

            var ECOKey = Request.Form["ECOKey"];
            var CardKey = Request.Form["CardKey"];

            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                StoreAttachAndComment(CardKey, updater);

                var tempinfo = new NebulaVM();
                if (string.Compare(Request.Form["ECOPartLifeCycleList"],"NONE",true) != 0)
                {
                    tempinfo.ECOPartLifeCycle = Request.Form["ECOPartLifeCycleList"];
                }
                if (string.Compare(Request.Form["GenericPartLifeCycleList"], "NONE", true) != 0)
                {
                    tempinfo.GenericPartLifeCycle = Request.Form["GenericPartLifeCycleList"];
                }
                tempinfo.UpdateMinipipCompleteInfo(CardKey);

                if (Request.Form["commitinfo"] != null)
                {
                    var redict = new RouteValueDictionary();
                    redict.Add("CardKey", CardKey);
                    return RedirectToAction("GoBackToCardByCardKey", "MiniPIP", redict);
                }

                if (Request.Form["forcecard"] == null)
                {
                    var allchecked = true;

                    if (string.IsNullOrEmpty(baseinfos[0].MCOIssued))
                    {
                        allchecked = false;
                        SetNoticeInfo("MCO number must be input on the ECO Pending card");
                    }
                    else if (string.IsNullOrEmpty(tempinfo.ECOPartLifeCycle))
                    {
                        allchecked = false;
                        SetNoticeInfo("ECO LifeCycle should not be empty");
                    }
                    else if (string.IsNullOrEmpty(tempinfo.GenericPartLifeCycle))
                    {
                        allchecked = false;
                        SetNoticeInfo("Generic LifeCycle should not be empty");
                    }
                    else if (string.Compare(tempinfo.ECOPartLifeCycle, tempinfo.GenericPartLifeCycle, true) != 0)
                    {
                        allchecked = false;
                        SetNoticeInfo("ECO LifeCycle is different from Generic LifeCycle");
                    }
                    else if ((string.Compare(baseinfos[0].ECOType, NebulaECOType.DVNS) == 0
                       || string.Compare(baseinfos[0].ECOType, NebulaECOType.RVNS) == 0)
                       && string.IsNullOrEmpty(baseinfos[0].FACustomerApproval))
                    {
                        allchecked = false;
                        SetNoticeInfo("FAI Approve Date must be inputed in ECO Signoff1/ECO Signoff2 card");
                    }

                    if (!allchecked)
                    {
                        var dict1 = new RouteValueDictionary();
                        dict1.Add("ECOKey", ECOKey);
                        dict1.Add("CardKey", CardKey);
                        return RedirectToAction(NebulaCardType.MiniPIPComplete, "MiniPIP", dict1);
                    }
                }

                NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.done);

                baseinfos[0].MiniPIPStatus = NebulaMiniPIPStatus.done;
                baseinfos[0].UpdateECO();
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", ECOKey);
            dict.Add("CardKey", CardKey);
            return RedirectToAction(NebulaCardType.MiniPIPComplete, "MiniPIP", dict);
        }

        public ActionResult DeleteCardAttachment(string CardKey, string FileName)
        {
            NebulaVM.DeleteCardAttachment(CardKey, FileName);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult RefreshCard(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            dict.Add("Refresh", "YES");
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ShowCardByCardKey(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteMiniPIP(string ECOKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                baseinfos[0].MiniPIPStatus = NebulaMiniPIPStatus.delete;
                baseinfos[0].UpdateECO();
            }
            return RedirectToAction("ViewAll", "MiniPIP");
        }

        public ActionResult ForceCompleteMiniPIP(string ECOKey)
        {
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            if (baseinfos.Count > 0)
            {
                if (string.Compare(baseinfos[0].CurrentECOProcess.ToUpper(), "COMPLETED") == 0)
                {
                    baseinfos[0].MiniPIPStatus = NebulaMiniPIPStatus.done;
                    baseinfos[0].UpdateECO();
                    SetNoticeInfo("Force complete MiniPIP "+ baseinfos[0].ECONum + " sucessfully !");
                    return RedirectToAction("CompletedMiniPIP", "MiniPIP");
                }
                else
                {
                    SetNoticeInfo("Fail to Force complete MiniPIP "+baseinfos[0].ECONum+". Current workflow process is "+ baseinfos[0].CurrentECOProcess + " not COMPLETED !");
                    return RedirectToAction("ViewAll", "MiniPIP");
                }
            }
            
            return RedirectToAction("ViewAll", "MiniPIP");
        }

        public ActionResult DeleteOrderInfo(string CardKey,string LineID)
        {
            NebulaVM.DeleteOrderInfo(CardKey, LineID);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOInfo(string CardKey, string WipJob)
        {
            NebulaVM.DeletJOInfo(CardKey, WipJob);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOEGCheck(string CardKey, string WipJob)
        {
            NebulaVM.DeletJOCheckInfo(CardKey, WipJob, NebulaJOCHECKTYPE.ENGTYPE);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteJOQACheck(string CardKey, string WipJob)
        {
            NebulaVM.DeletJOCheckInfo(CardKey, WipJob, NebulaJOCHECKTYPE.QATYPE);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        private void CreateRVSCards(string ECOKey)
        {
            var realcardkey = string.Empty;

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOSignoff2, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.CustomerApprovalHold, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());

        }

        private void CreateRVNSCards(string ECOKey)
        {
            var realcardkey = string.Empty;

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOSignoff2, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.CustomerApprovalHold, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.ECOComplete, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleOrdering, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleBuilding, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleShipment, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.SampleCustomerApproval, currenttime.ToString());

            currenttime = currenttime.AddMinutes(1);
            realcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), NebulaCardType.MiniPIPComplete, currenttime.ToString());

        }

        public ActionResult RollBack2ThisCard(string CardKey, string ECOKey)
        {
            
            var baseinfos = ECOBaseInfo.RetrieveECOBaseInfo(ECOKey);
            var vm = NebulaVM.RetrieveCard(CardKey);

            NebulaVM.RollBack2This(ECOKey, CardKey);

            new System.Threading.ManualResetEvent(false).WaitOne(2000);
            var currenttime = DateTime.Now;
            currenttime = currenttime.AddMinutes(1);

            if (string.Compare(vm[0].CardType, NebulaCardType.ECOPending) == 0)
            {
                var newcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), vm[0].CardType, currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", vm[0].ECOKey);
                dict.Add("CardKey", newcardkey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVS) == 0)
            {
                CreateRVSCards(ECOKey);

                var spcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], vm[0].CardType);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", spcard[0].CardKey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else if (string.Compare(baseinfos[0].ECOType, NebulaECOType.RVNS) == 0)
            {
                CreateRVNSCards(ECOKey);

                var spcard = NebulaVM.RetrieveSpecialCard(baseinfos[0], vm[0].CardType);
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", ECOKey);
                dict.Add("CardKey", spcard[0].CardKey);
                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            else
            {
                var newcardkey = NebulaVM.CreateCard(ECOKey, NebulaVM.GetUniqKey(), vm[0].CardType, currenttime.ToString());
                var dict = new RouteValueDictionary();
                dict.Add("ECOKey", vm[0].ECOKey);
                dict.Add("CardKey", newcardkey);

                if (string.Compare(baseinfos[0].ECOType, NebulaECOType.DVNS) == 0
                    || string.Compare(baseinfos[0].ECOType, NebulaECOType.DVS) == 0)
                {
                    var templist = NebulaVM.RetrieveECOCards(baseinfos[0]);
                    if (templist.Count > 0)
                    {
                        CreateAllDefaultCards(templist.Count, baseinfos[0]);
                    }
                }

                return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
            }
            
        }

        public ActionResult DeleteJOStore(string CardKey, string WipJob)
        {
            NebulaVM.DeletJOStore(CardKey, WipJob);

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);

        }

        public ActionResult GoBackToCardByCardKey(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult DeleteSPCardComment(string CardKey, string Date)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);

            NebulaVM.DeleteSPCardComment(CardKey, Date);

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ModifyCardComment(string CardKey, string Date)
        {
            var vm = NebulaVM.RetrieveSPCardComment(CardKey, Date);
            if (!string.IsNullOrEmpty(vm.CardKey))
                return View(vm);
            else
                return RedirectToAction("ViewAll", "MiniPIP");
        }

        [HttpPost, ActionName("ModifyCardComment")]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyCardCommentPost()
        {
            var CardKey = Request.Form["HCardKey"];
            var Date = Request.Form["HDate"];

            if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
            {
                var comm = new ECOCardComments();
                comm.Comment = SeverHtmlDecode.Decode(this, Request.Form["commenteditor"]);
                NebulaVM.UpdateSPCardComment(CardKey, Date, comm.dbComment);
            }

            var vm = NebulaVM.RetrieveCard(CardKey);
            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileFileDownload(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                NebulaDataCollector.DownloadAgile(ecolist, this, NebulaAGILEDOWNLOADTYPE.ATTACH);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileFileNameDownload(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                NebulaDataCollector.DownloadAgile(ecolist, this, NebulaAGILEDOWNLOADTYPE.ATTACHNAME);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult ReNewCard(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);

            NebulaVM.UpdateCardStatus(CardKey, NebulaCardStatus.working);

            if (string.Compare(vm[0].CardType, NebulaCardType.MiniPIPComplete) == 0)
            {
                var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
                if (baseinfo.Count > 0)
                {
                    baseinfo[0].MiniPIPStatus = NebulaMiniPIPStatus.working;
                    baseinfo[0].UpdateECO();
                }
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        public ActionResult AgileWorkFlowDownload(string CardKey)
        {
            var vm = NebulaVM.RetrieveCard(CardKey);

            var baseinfo = ECOBaseInfo.RetrieveECOBaseInfo(vm[0].ECOKey);
            if (baseinfo.Count > 0)
            {
                var ecolist = new List<string>();
                ecolist.Add(baseinfo[0].ECONum);
                NebulaDataCollector.DownloadAgile(ecolist, this, NebulaAGILEDOWNLOADTYPE.WORKFLOW);
            }

            var dict = new RouteValueDictionary();
            dict.Add("ECOKey", vm[0].ECOKey);
            dict.Add("CardKey", vm[0].CardKey);
            return RedirectToAction(vm[0].CardType, "MiniPIP", dict);
        }

        private void StoreAgileAttch(string ECONUM,List<NebulaVM> vm)
        {
            var syscfgdict = CfgUtility.GetSysConfig(this);
            var dir = syscfgdict["SAVELOCATION"] +"\\" + ECONUM;
            if (Directory.Exists(dir))
            {
                //string datestring = DateTime.Now.ToString("yyyyMMdd");
                string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + ECONUM + "\\";
                if (!Directory.Exists(imgdir))
                {
                    Directory.CreateDirectory(imgdir);
                }


                var cardinfo = NebulaVM.RetrieveSignoffInfo(vm[0].CardKey);

                var files = Directory.EnumerateFiles(dir);
                foreach (var fl in files)
                {
                    var fn = Path.GetFileName(fl);
                    var desfn = imgdir + fn;
                    var url = "/userfiles/docs/" + ECONUM + "/" + fn;

                    if (fl.ToUpper().Contains("QR_")
                        || fl.ToUpper().Contains("QUALIFICATION"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.ECOQRFile.Contains(fn))
                        {
                            cardinfo.ECOQRFile = cardinfo.ECOQRFile + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("PEER")
                        && fl.ToUpper().Contains("REVIEW"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.EEPROMPeerReview.Contains(fn))
                        {
                            cardinfo.EEPROMPeerReview = cardinfo.EEPROMPeerReview + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("TRACEVIEW"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.ECOTraceview.Contains(fn))
                        {
                            cardinfo.ECOTraceview = cardinfo.ECOTraceview + url + ":::";
                        }
                    }
                    else if (fl.ToUpper().Contains("COMPARE"))
                    {
                        System.IO.File.Copy(fl, desfn, true);
                        if (!cardinfo.SpecCompresuite.Contains(fn))
                        {
                            cardinfo.SpecCompresuite = cardinfo.SpecCompresuite + url + ":::";
                        }
                    }
                }

                cardinfo.UpdateSignoffInfo(vm[0].CardKey);
            }
        }

        public ActionResult AgileAttach(string ECONUM)
        {
            var ecoinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONUM);
            foreach (var ecoitem in ecoinfos)
            {
                var vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.ECOSignoff1);
                if (vm.Count > 0)
                {
                    StoreAgileAttch(ECONUM,vm);

                    if (string.Compare(vm[0].CardStatus, NebulaCardStatus.working) == 0)
                    {
                        NebulaVM.UpdateCardStatus(vm[0].CardKey, NebulaCardStatus.pending);
                    }
                }
                else
                {
                    vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.ECOSignoff2);
                    if (vm.Count > 0)
                    {
                        StoreAgileAttch(ECONUM, vm);

                        if (string.Compare(vm[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(vm[0].CardKey, NebulaCardStatus.pending);
                        }
                    }
                }
            }
            return View();
        }


        public ActionResult AgileWorkFlow(string ECONUM)
        {
            var ecoinfos = ECOBaseInfo.RetrieveECOBaseInfoWithECONum(ECONUM);
            var workflowinfo = NebulaDataCollector.RetrieveAgileWorkFlowData(ECONUM, this);

            foreach (var ecoitem in ecoinfos)
            {
                    if(!string.IsNullOrEmpty(workflowinfo.CurrentProcess))
                    {
                    ecoitem.CurrentECOProcess = workflowinfo.CurrentProcess;
                    ecoitem.CurrentFlowType = workflowinfo.WorkFlowType;
                    ecoitem.UpdateECO();
                    }

                    var vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.ECOSignoff1);
                    if (vm.Count > 0)
                    {
                        var cardinfo = NebulaVM.RetrieveSignoffInfo(vm[0].CardKey);
                        cardinfo.ECOMDApprover = workflowinfo.ECOMDApprover;
                        cardinfo.ECOTRApprover = workflowinfo.ECOTRApprover;
                        cardinfo.UpdateSignoffInfo(vm[0].CardKey);

                        if (string.Compare(vm[0].CardStatus, NebulaCardStatus.working) == 0)
                        {
                            NebulaVM.UpdateCardStatus(vm[0].CardKey, NebulaCardStatus.pending);
                        }
                    }
                    else
                    {
                        vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.ECOSignoff2);
                        if (vm.Count > 0)
                        {
                            var cardinfo = NebulaVM.RetrieveSignoffInfo(vm[0].CardKey);
                            cardinfo.ECOMDApprover = workflowinfo.ECOMDApprover;
                            cardinfo.ECOTRApprover = workflowinfo.ECOTRApprover;
                            cardinfo.UpdateSignoffInfo(vm[0].CardKey);

                            if (string.Compare(vm[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(vm[0].CardKey, NebulaCardStatus.pending);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(workflowinfo.CApproveHoldDate))
                    {
                        vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.CustomerApprovalHold);
                        if (vm.Count > 0)
                        {
                            var cardinfo = NebulaVM.RetrieveCustomerApproveHoldInfo(vm[0].CardKey);
                            cardinfo.ECOCustomerHoldStartDate = ConvertUSLocalToDate(workflowinfo.CApproveHoldDate);
                            cardinfo.UpdateCustomerApproveHoldStartDate(vm[0].CardKey);
                        }
                    }

                    if (!string.IsNullOrEmpty(workflowinfo.ECOCompleteDate))
                    {
                        vm = NebulaVM.RetrieveSpecialCard(ecoitem, NebulaCardType.ECOComplete);
                        if (vm.Count > 0)
                        {
                            vm[0].ECOCompleted = NebulaYESNO.YES;
                            vm[0].ECOCompleteDate = ConvertUSLocalToDate(workflowinfo.ECOCompleteDate);
                            vm[0].UpdateECOCompleteInfo(vm[0].CardKey);

                            if (string.Compare(vm[0].CardStatus, NebulaCardStatus.working) == 0)
                            {
                                NebulaVM.UpdateCardStatus(vm[0].CardKey, NebulaCardStatus.pending);
                            }
                        }
                    }
                }

            return View();
        }


        private List<string> ReceiveRMAFiles()
        {
            var ret = new List<string>();

            try
            {
                foreach (string fl in Request.Files)
                {
                    if (fl != null && Request.Files[fl].ContentLength > 0)
                    {
                        string fn = Path.GetFileName(Request.Files[fl].FileName)
                            .Replace(" ", "_").Replace("#", "").Replace("'", "")
                            .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                        string datestring = DateTime.Now.ToString("yyyyMMdd");
                        string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";

                        if (!Directory.Exists(imgdir))
                        {
                            Directory.CreateDirectory(imgdir);
                        }

                        fn = Path.GetFileNameWithoutExtension(fn) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fn);
                        Request.Files[fl].SaveAs(imgdir + fn);

                        var url = "/userfiles/docs/" + datestring + "/" + fn;

                        ret.Add(url);
                    }
                }

            }
            catch (Exception ex)
            { return ret; }

            return ret;
        }

        private void StoreAttachAndComment(string CardKey, string updater, NebulaVM cardinfo = null)
        {
            var urls = ReceiveRMAFiles();

            if (!string.IsNullOrEmpty(Request.Form["attachmentupload"]))
            {
                var internalreportfile = Request.Form["attachmentupload"];
                var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                    .Replace(" ", "_").Replace("#", "").Replace("'", "")
                    .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                var url = "";
                foreach (var r in urls)
                {
                    if (r.Contains(originalname))
                    {
                        url = r;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(url))
                {
                    NebulaVM.StoreCardAttachment(CardKey, url);
                }
            }

            if (cardinfo != null)
            {
                if (!string.IsNullOrEmpty(Request.Form["qrfileupload"]))
                {
                    var internalreportfile = Request.Form["qrfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.ECOQRFile = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["peerfileupload"]))
                {
                    var internalreportfile = Request.Form["peerfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.EEPROMPeerReview = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["traceviewfileupload"]))
                {
                    var internalreportfile = Request.Form["traceviewfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.ECOTraceview = url;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["speccomfileupload"]))
                {
                    var internalreportfile = Request.Form["speccomfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.SpecCompresuite = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["codefileupload"]))
                {
                    var internalreportfile = Request.Form["codefileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.AgileCodeFile = url;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["specfileupload"]))
                {
                    var internalreportfile = Request.Form["specfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.AgileSpecFile = url;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Form["testingfileupload"]))
                {
                    var internalreportfile = Request.Form["testingfileupload"];
                    var originalname = Path.GetFileNameWithoutExtension(internalreportfile)
                        .Replace(" ", "_").Replace("#", "").Replace("'", "")
                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");

                    var url = "";
                    foreach (var r in urls)
                    {
                        if (r.Contains(originalname))
                        {
                            url = r;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        cardinfo.AgileTestFile = url;
                    }
                }
            }


            if (!string.IsNullOrEmpty(Request.Form["commenteditor"]))
            {
                var rootcause = Server.HtmlDecode(Request.Form["commenteditor"]);
                var dbstr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(rootcause));
                NebulaVM.StoreCardComment(CardKey, dbstr, updater, DateTime.Now.ToString());
            }

        }

        private static void logreportinfo(string filename, string info)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    var content = System.IO.File.ReadAllText(filename);
                    content = content + info;
                    System.IO.File.WriteAllText(filename, content);
                }
                else
                {
                    System.IO.File.WriteAllText(filename, info);
                }
            }
            catch (Exception ex) { }
        }

    }
}