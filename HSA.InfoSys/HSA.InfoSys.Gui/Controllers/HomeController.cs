﻿namespace HSA.InfoSys.Gui.Controllers
{
    using System.Web.Mvc;
    using log4net;

    [HandleError]
    public class HomeController : Controller
    {
        private CrawlControllerClient client = new CrawlControllerClient();
        private static readonly ILog log = Logging.Logging.GetLogger("Gui");

        [Authorize]
        public ActionResult Index()
        {
            ViewData["navid"] = "home";
            ViewData["label1"] = "System Setup";

            client.StartSearch();

            // Test
            //MySqlConnection connection = new MySqlConnection("server=infosys.informatik.hs-augsburg.de;uid=root;pwd=goqu!ae0Ah;database=infosys");
            //connection.Open();


            return View();
        }

        public ActionResult About()
        {
            ViewData["navid"] = "about";
            ViewData["message"] = "About Action";

            client.StartSearch();

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
            /*
            IDBManager dbm = new DBManager.DBManager();

            Component comp = new Component { componentGUID = System.Guid.NewGuid(), name = "abc", category = "hardware", componentId = 12 };

            dbm.addNewObject(comp);
             */


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