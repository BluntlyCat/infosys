// ------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.Web.Mvc;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// The controller for the home page.
    /// </summary>
    [HandleError]
    public class HomeController : Controller
    {
        /// <summary>
        /// The logger for the home controller
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("HomeController");

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            this.ViewData["navid"] = "home";
            this.ViewData["label1"] = Properties.Resources.TEST_LABLE1;

            return this.View();
        }

        /// <summary>
        /// Shows the about site.
        /// </summary>
        /// <returns>The result of this action.</returns>
        public ActionResult About()
        {
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
    }
}
