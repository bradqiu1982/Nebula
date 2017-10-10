using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nebula.Controllers
{
    public class ProLegoController : Controller
    {
        // GET: ProLego
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RoleList()
        {
            var res = new JsonResult();
            var list = new List<string>();
            list.Add("PQE");
            list.Add("PE");
            list.Add("CQE");
            list.Add("ME");
            list.Add("TE");
            res.Data = list;
            return res;
        }

        [HttpPost]
        public JsonResult AddRole()
        {
            var rolename = Request.Form["role"];
            //save role



            var res = new JsonResult();
            res.Data = new { success = true };

            return res;
        }
    }
}
