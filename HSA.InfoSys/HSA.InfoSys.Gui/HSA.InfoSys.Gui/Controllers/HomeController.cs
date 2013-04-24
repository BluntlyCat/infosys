using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HSA.InfoSys.Gui.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        //private static readonly ILog log = Logging.Logging.GetLogger("Gui");

        [Authorize]
        public ActionResult Index()
        {
            ViewData["navid"] = "home";
            ViewData["label1"] = "System Setup";

            // Test
            //MySqlConnection connection = new MySqlConnection("server=infosys.informatik.hs-augsburg.de;uid=root;pwd=goqu!ae0Ah;database=infosys");
            //connection.Open();


            return View();
        }

        public ActionResult About()
        {
            ViewData["navid"] = "about";
            ViewData["message"] = "About Action";

            return View();
        }

        public ActionResult Contact()
        {
            ViewData["navid"] = "contact";
            ViewData["message"] = "Contact Action";

            //log.Debug("jemand will kontakt aufnehmen");

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchResult()
        {
            ViewData["navid"] = "home";

            // get POST data from form
            string[] components = Request["components[]"].Split(',');

            //string test = Request.Form["components"];

            // vars to view
            ViewData["components"] = components;

            return View();
        }
    }
}
