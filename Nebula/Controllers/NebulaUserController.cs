using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Web.Routing;
using System.Collections.Specialized;
using Nebula.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
using System.Web.Http.Results;

namespace Nebula.Controllers
{
    public class NebulaUserController : Controller
    {
        public ActionResult UserLogin()
        {
            return View();
        }

        public JsonResult UserLoginPost()
        {
            var loginid = Request.Form["loginid"];
            var loginpwd = Request.Form["loginpwd"];

            var dbret = NebulaUserViewModels.RetrieveUser(loginid);
            if (dbret == null)
            {
                if (string.Compare(loginpwd, "abc@123", true) == 0)
                {
                    var user = new NebulaUserViewModels();
                    user.Email = loginid;
                    user.Password = loginpwd;
                    user.RegistUser();

                    var ck = new Dictionary<string, string>();
                    ck.Add("logonuser", loginid);
                    CookieUtility.SetCookie(this, ck);

                    var res = new JsonResult();
                    res.Data = new { success = true };
                    return res;
                }
                else
                {
                    var res = new JsonResult();
                    res.Data = new { success = false,msg = "User Not Exist!" };
                    return res;
                }
            }

            if (string.Compare(loginpwd, dbret.Password, true) == 0)
            {

                var ck = new Dictionary<string, string>();
                ck.Add("logonuser", loginid);
                CookieUtility.SetCookie(this, ck);

                var res = new JsonResult();
                res.Data = new { success = true };
                return res;
            }
            else
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "Password Wrong!" };
                return res;
            }
                
        }

        public JsonResult ResetPWD()
        {
            var loginid = Request.Form["loginid"];
            var dbret = NebulaUserViewModels.RetrieveUser(loginid);
            if (dbret == null)
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "User Not Exist!" };
                return res;
            }

            SendResetEmail(loginid);
            var res1 = new JsonResult();
            res1.Data = new { success = true, msg = "We have send you an email, please change your password from the email!" };
            return res1;
        }

        private void SendResetEmail(string username)
        {
            string updatetime = DateTime.Now.ToString();
            var routevalue = new RouteValueDictionary();
            routevalue.Add("resetstr", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(username + "||" + updatetime)));

            //send validate email
            string scheme = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
            string validatestr = this.Url.Action("ResetPWDLink", "NebulaUser", routevalue, scheme);
            var netcomputername = "";
            try { netcomputername = System.Net.Dns.GetHostName(); }
            catch (Exception ex) { }
            validatestr = validatestr.Replace("//localhost", "//" + netcomputername);

            var toaddrs = new List<string>();
            toaddrs.Add(username);
            EmailUtility.SendEmail(this, "BR Trace System - Reset Password", toaddrs, validatestr);
        }

        public ActionResult ResetPWDLink(string resetstr)
        {
            if (resetstr != null)
            {
                var bs = Convert.FromBase64String(resetstr);
                var val = UTF8Encoding.UTF8.GetString(bs);
                ViewBag.UserName = val.Split(new string[] { "||" }, StringSplitOptions.None)[0];
            }
            return View();
        }

        public JsonResult ResetPWDLinkPost()
        {
            var loginid = Request.Form["loginid"];
            var loginpwd = Request.Form["loginpwd"];

            var dbret = NebulaUserViewModels.RetrieveUser(loginid);
            if (dbret == null)
            {
                var res = new JsonResult();
                res.Data = new { success = false, msg = "User Not Exist!" };
                return res;
            }
            else
            {
                var ck = new Dictionary<string, string>();
                ck.Add("logonuser", loginid);
                CookieUtility.SetCookie(this, ck);

                NebulaUserViewModels.RestPwd(loginid, loginpwd);
                var res = new JsonResult();
                res.Data = new { success = true, msg = "" };
                return res;
            }

        }

    }
}