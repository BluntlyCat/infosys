// ------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using System;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// The controller for the system.
    /// </summary>
    [HandleError]
    public class SystemController : Controller
    {
        ILog log = Logging.GetLogger("GuiLogger");
 
        /// <summary>
        /// Called when the home page is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            this.ViewData["navid"] = "mysystems";
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult IndexSubmit()
        {
            // get name of the new system
            string newsystem = Request["newsystem"];

            //// init
            //cc = CrawlController.ClientProxy;

            //// log
            //log.Info("add new system");

            //// get id of current logged-in user
            //MembershipUser membershipUser = Membership.GetUser();
            //string UserID = membershipUser.ProviderUserKey.ToString();
            //int id = Convert.ToInt32(UserID);

            //// save to db
            //Guid guid;
            //var system = cc.CreateSystemService(id, null, null);
            //guid = cc.AddEntity(system);

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// Called when page components is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Components()
        {
            this.ViewData["navid"] = "mysystems";
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ComponentsSubmit()
        {
            // get POST data from form
            string[] components = Request["components[]"].Split(',');

            foreach (string comp in components)
            {
                // create new GUID
                string guid = Guid.NewGuid().ToString();

                // save component to DB
                // #TODO
            }

            // vars to view
            //this.ViewData["components"] = components;

            return this.RedirectToAction("Components", "System");
        }

        /// <summary>
        /// Called when page search is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult SearchConfig()
        {
            this.ViewData["navid"] = "mysystems";

            MembershipUser user = Membership.GetUser();
            this.ViewData["useremail"] = user.Email;

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchConfigSubmit()
        {
            System.Web.HttpRequestBase r = Request;

            string schedulerOn = Request["schedulerOn"];
            string emailsOn = Request["emailsOn"];
            string websitesOn = Request["websitesOn"];

            string sc_days = Request["sc_days"];
            string sc_time = Request["sc_time"];
            string sc_date = Request["sc_date"];

            string[] emails = Request["emails[]"].Split(',');
            string[] websites = Request["websites[]"].Split(',');

            // #TODO save to db

            return this.RedirectToAction("SearchConfig", "System");
        }
    }
}
