namespace HSA.InfoSys.Gui.Controllers
{
    using System.Web.Mvc;
    using log4net;
    using HSA.InfoSys.Common.Logging;

    [HandleError]
    public class HomeController : Controller
    {
        private static readonly ILog log = Logging.GetLogger("Gui");
        private CrawlControllerClient client = new CrawlControllerClient();

        [Authorize]
        public ActionResult Index()
        {
            ViewData["navid"] = "home";
            ViewData["label1"] = Properties.Resources.TEST_LABLE1;

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
            
            /*
            
            // Beispiel nhibernate
            IDBManager dbm = new DBManager.DBManager();

            Component comp = new Component { componentGUID = System.Guid.NewGuid(), name = "abc", category = "hardware", componentId = 12 };

            dbm.addNewObject(comp);
             * */

            /*
            // Beispiel, zugriff über wcf
            CrawlControllerClient client = new CrawlControllerClient();
            client.
             * */
            


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
