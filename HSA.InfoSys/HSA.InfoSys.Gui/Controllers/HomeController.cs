// ------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System.Web.Mvc;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using System;

    /// <summary>
    /// The controller for the home page.
    /// </summary>
    [HandleError]
    public class HomeController : Controller
    {
        /// <summary>
        /// The logger for the home controller
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("HomeController");

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            this.ViewData["navid"] = "home";
            this.ViewData["label1"] = Properties.Resources.TEST_LABLE1;

            //DateTime date1 = new DateTime(1970, 1, 1);  //Refernzdatum (festgelegt)
            //DateTime date2 = DateTime.Now;              //jetztiges Datum / Uhrzeit
            //TimeSpan ts = new TimeSpan(date2.Ticks - date1.Ticks);  // das Delta ermitteln
            //// Das Delta als gesammtzahl der sekunden ist der Timestamp
            //string timestamp = Convert.ToInt32(ts.TotalSeconds);

            TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1));
            int timestamp = (int)ts.TotalSeconds;
            Console.WriteLine(timestamp);

            System.DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dt = dt.AddSeconds(timestamp);
            Console.WriteLine(dt);


            //// Test
            //// MySqlConnection connection = new MySqlConnection("server=infosys.informatik.hs-augsburg.de;uid=root;pwd=goqu!ae0Ah;database=infosys");
            //// connection.Open();

            return this.View();
        }

        /// <summary>
        /// Shows the about site.
        /// </summary>
        /// <returns>The result of this action.</returns>
        public ActionResult About()
        {
            Log.Info("Testlog in Database");

            this.ViewData["navid"] = "about";
            this.ViewData["message"] = "About Action";

            return this.View();
        }

        /// <summary>
        /// Shows the contact site.
        /// </summary>
        /// <returns>The result of this action.</returns>
        public ActionResult Contact()
        {
            this.ViewData["navid"] = "contact";
            this.ViewData["message"] = "Contact Action";

            //// log.Debug("jemand will kontakt aufnehmen");

            return this.View();
        }

        /// <summary>
        /// Shows the search results.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult SearchResult()
        {
            /*
            // Beispiel, zugriff über wcf
            CrawlControllerClient client = new CrawlControllerClient();

            var comp = client.CreateComponent("abc", "nutzloser Kram");

            client.AddEntity(comp);
            */

            this.ViewData["navid"] = "home";

            // get POST data from form
            string[] components = Request["components[]"].Split(',');

            // string test = Request.Form["components"];

            // vars to view
            this.ViewData["components"] = components;

            return this.View();
        }
    }
}
