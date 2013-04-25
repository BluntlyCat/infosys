using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HSA.InfoSys.Gui.Controllers
{
    [HandleError]
    public class SystemController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData["navid"] = "mysystems";
            return View();
        }

        [Authorize]
        public ActionResult Components()
        {
            ViewData["navid"] = "mysystems";
            return View();
        }

        [Authorize]
        public ActionResult SearchConfig()
        {
            ViewData["navid"] = "mysystems";
            return View();
        }

    }
}
