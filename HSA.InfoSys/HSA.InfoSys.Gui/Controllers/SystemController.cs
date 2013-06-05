// ------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Services;
    using HSA.InfoSys.Common.Services.Data;

    /// <summary>
    /// The controller for the system.
    /// </summary>
    [HandleError]
    public class SystemController : Controller
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SystemController");
 
        /// <summary>
        /// Called when the home page is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            var cc = CrawlControllerClient<IDBManager>.ClientProxy;
            // get id of current logged-in user
            MembershipUser membershipuser = Membership.GetUser();
            string userid = membershipuser.ProviderUserKey.ToString();
            int id = Convert.ToInt32(userid);

            IList<OrgUnit> orgUnits =  cc.GetOrgUnitsByUserID(id);

            this.ViewData["orgUnits"] = orgUnits;

            this.ViewData["navid"] = "mysystems";

            // Testing -> Dummy SystemGUID
            // Da wir über das WCF noch keine OrgUnits abfragen können, haben wir zum Testen ein
            // ein OrgUnit erzeugt, und dessen GUID aus der DB hergenommen und hier eingefügt, sodass
            // wenn wir in der GUI auf den Edit-Button eines Systems klicken, diese GUID mitgeliefert wird, 
            // und wir auf der Editor-Seite über diese GUID das Objekt aus der DB holen können.
            this.ViewData["systemguid"] = "05e1b084-535d-4bd9-b2d2-a1c800c87161";

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult IndexSubmit()
        {
            // get name of the new system
            string newsystem = Request["newsystem"];

            // init
            var cc = CrawlControllerClient<IDBManager>.ClientProxy;

            // log
            Log.Info("add new system");

            // get id of current logged-in user
            MembershipUser membershipuser = Membership.GetUser();
            string userid = membershipuser.ProviderUserKey.ToString();
            int id = Convert.ToInt32(userid);

            // create Scheduler
            var scheduler = cc.CreateScheduler(3, 0);

            // create SystemConfig
            var systemConfig = cc.CreateOrgUnitConfig(null, null, false, false, false, scheduler);

            // create System
            var system = cc.CreateOrgUnit(id, newsystem);
            system.OrgUnitConfig = systemConfig;

            // save to db
            Guid guid = cc.AddEntity(system);

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// Called when page components is loading.
        /// </summary>
        /// <param name="systemGUID">The systemGUID.</param>
        /// <returns>
        /// The result of this action.
        /// </returns>
        [Authorize]
        [HttpGet]
        public ActionResult Components(string systemGUID)
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            this.ViewData["navid"] = "mysystems";
            this.ViewData["systemguid"] = systemguid;

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
        /// <param name="systemGUID">The systemGUID.</param>
        /// <returns>
        /// The result of this action.
        /// </returns>
        [Authorize]
        [HttpGet]
        public ActionResult SearchConfig(string systemGUID)
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // init
            var cc = CrawlControllerClient<IDBManager>.ClientProxy;

            // get SystemConfig, OrgUnitConfig, Scheduler
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnitConfig", "Scheduler")) as OrgUnit;
            var config = orgUnit.OrgUnitConfig;
            var scheduler = config.SchedulerTime;

            // set all config data for view
            this.ViewData["schedulerActive"] = config.SchedulerActive;
            this.ViewData["emailActive"] = config.EmailActive;
            this.ViewData["urlActive"] = config.URLActive;

            this.ViewData["sc_days"] = scheduler.Days;
            this.ViewData["sc_hours"] = scheduler.Hours;
            this.ViewData["sc_begin"] = scheduler.Begin;

            this.ViewData["emails"] = config.Emails;
            this.ViewData["urls"] = config.URLS;

            //MembershipUser user = Membership.GetUser();
            //this.ViewData["useremail"] = user.Email;

            this.ViewData["navid"] = "mysystems";
            this.ViewData["systemguid"] = systemguid;

            return this.View();
        }

        /// <summary>
        /// Called on submitting the config search.
        /// </summary>
        /// <returns>The action result.</returns>
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
