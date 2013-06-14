// ------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Security;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using Newtonsoft.Json;
    using HSA.InfoSys.Common.Services.LocalServices;

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
        /// The controller host for WCF service.
        /// </summary>
        private static WCFControllerHost ControllerHost;

        /// <summary>
        /// The search recall.
        /// </summary>
        private static SearchRecall SearchRecall;

        /// <summary>
        /// The search finished
        /// </summary>
        private static bool? SearchFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemController"/> class.
        /// </summary>
        public SystemController()
        {
            if (ControllerHost == null && SearchRecall == null)
            {
                Addresses.Initialize();
                ControllerHost = new WCFControllerHost();
                SearchRecall = ControllerHost.OpenWCFHost<SearchRecall, ISearchRecall>(SearchRecall.SearchRecallFactory);
                SearchRecall.OnRecall += new SearchRecall.RecallHandler(SearchRecall_OnRecall);
            }
        }
 
        /// <summary>
        /// Called when the home page is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            var cc = WCFControllerClient<IDBManager>.ClientProxy;
            // get id of current logged-in user
            MembershipUser membershipuser = Membership.GetUser();
            string userid = membershipuser.ProviderUserKey.ToString();
            int id = Convert.ToInt32(userid);

            IList<OrgUnit> orgUnits =  cc.GetOrgUnitsByUserID(id);
            

            this.ViewData["orgUnits"] = orgUnits;

            this.ViewData["navid"] = "mysystems";

            this.ViewData["searchFinished"] = SearchFinished;

            return this.View();
        }

        /// <summary>
        /// Is Called when a new OrgUnit was created
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult IndexSubmit()
        {
            // get name of the new system
            string newsystem = Request["newsystem"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            // log
            Log.Info("add new system");

            // get id of current logged-in user
            MembershipUser membershipuser = Membership.GetUser();
            string userid = membershipuser.ProviderUserKey.ToString();
            int id = Convert.ToInt32(userid);

            // create SystemConfig
            var systemConfig = cc.CreateOrgUnitConfig(null, null, false, false, 1, 12, new DateTime(), false);

            // create System
            var system = cc.CreateOrgUnit(id, newsystem);
            system.OrgUnitConfig = systemConfig;

            // save to db
            cc.AddEntity(system);

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// Deletes the system.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteSystem()
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            //get orgUnit
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnit", "OrgUnitConfig")) as OrgUnit;

            // get all components by OrgUnitId
            var components = cc.GetComponentsByOrgUnitId(new Guid(systemguid));

            
            // entities must be deleted in this order because of db dependencies
            // delete all components and their results 
            foreach (Component comp in components)
            {
                cc.DeleteEntity(comp);
            }

            //delete OrgUnit and orgUnitConfig
            cc.DeleteEntity(orgUnit);
            cc.DeleteEntity(orgUnit.OrgUnitConfig);

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// starts the real time search
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult RealTimeSearch()
        {
            string systemguid = Request.QueryString["sysguid"];
            SearchFinished = false;

            //trigger search
            WCFControllerClient<ISolrController>.ClientProxy.SearchForOrgUnit(Guid.Parse(systemguid));

            //start thread which is watching the list of searches
            SearchRecall.StartService();

            return this.RedirectToAction("Index", "System");
        }


        /// <summary>
        /// Occurs when [recall] when all searches finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        void SearchRecall_OnRecall(object sender)
        {
            SearchRecall.StopService(true);
            SearchFinished = true;

            Server.Transfer(this.Request.Path);
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
        public ActionResult Components()
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            //get OrgUnit
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnit")) as OrgUnit;

            // get all components by OrgUnitId
            var components = cc.GetComponentsByOrgUnitId(new Guid(systemguid));

            this.ViewData["navid"] = "mysystems";
            this.ViewData["systemguid"] = systemguid;
            this.ViewData["orgUnitName"] = orgUnit.Name;

            if (components.Count > 0)
            {
                this.ViewData["components"] = components;
            }

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ComponentsSubmit()
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // get POST data from form
            string component = Request["components"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            // get orgUnit by id
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnit")) as OrgUnit;
            
            // log
            Log.Info("add new component");

            // save component to DB
            cc.AddEntity(cc.CreateComponent(component, orgUnit));

            return this.Redirect("/System/Components?sysguid=" + systemguid);

        }

        [Authorize]
        [HttpGet]
        public ActionResult DeleteComponent()
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // get compid from GET-Request
            string compid = Request.QueryString["compid"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            // get component by id
            var component = cc.GetEntity(new Guid(compid), cc.LoadThisEntities("Component"));
            
            // delete component
            cc.DeleteEntity(component);

            return this.Redirect("/System/Components?sysguid=" + systemguid);
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
        public ActionResult SearchConfig()
        {
            // get systemguid from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            // get id of current logged-in user
            MembershipUser membershipuser = Membership.GetUser();
            string userid = membershipuser.ProviderUserKey.ToString();
            int id = Convert.ToInt32(userid);

            IList<OrgUnit> orgUnits = cc.GetOrgUnitsByUserID(id);

            OrgUnit delItem = null;

            foreach (var item in orgUnits)
            {
                if (item.EntityId == new Guid(systemguid))
                {
                     delItem = item;
                }
            }

            orgUnits.Remove(delItem);
            this.ViewData["orgUnits"] = orgUnits;


            // get SystemConfig, OrgUnitConfig
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
            var config = orgUnit.OrgUnitConfig;

            // set all config data for view
            this.ViewData["schedulerActive"] = config.SchedulerActive;
            this.ViewData["emailActive"] = config.EmailActive;
            this.ViewData["urlActive"] = config.URLActive;

            this.ViewData["sc_days"] = config.Days;
            this.ViewData["sc_hours"] = config.Time;

            if (!String.IsNullOrEmpty(config.Emails))
            {
                this.ViewData["emails"] = JsonConvert.DeserializeObject<string[]>(config.Emails);
            }
            else
            {
                this.ViewData["emails"] = new string[0];
            }

            if (!String.IsNullOrEmpty(config.URLS))
            {
                this.ViewData["urls"] = JsonConvert.DeserializeObject<string[]>(config.URLS);
            } 
            else 
            {
                this.ViewData["urls"] = new string[0];
            }
            

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
            // get data from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            // get SystemConfig, OrgUnitConfig
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
            var config = orgUnit.OrgUnitConfig;

            // set schedulerOn, days, time
            if (Request["schedulerOn"] == "on")
            {
                string sc_days = Request["sc_days"];
                string sc_time = Request["sc_time"];

                config.SchedulerActive = true;
                config.Days = Convert.ToInt32(sc_days);
                config.Time = Convert.ToInt32(sc_time);

                //TODO: vergleich der zeiten
            }
            else
            {
                config.SchedulerActive = false;
            }

            // set Emails
            if (!String.IsNullOrEmpty(Request["emails[]"]))
            {
                string[] emails = Request["emails[]"].Split(',');
                config.Emails = JsonConvert.SerializeObject(emails);
            }
            else
            {
                config.Emails = null;
            }

            // set emailsOn
            if (Request["emailsOn"] == "on")
            {
                config.EmailActive = true;
            }
            else
            {
                config.EmailActive = false;
            }

            // set Websites
            if (!String.IsNullOrEmpty(Request["websites[]"]))
            {
                string[] websites = Request["websites[]"].Split(',');
                config.URLS = JsonConvert.SerializeObject(websites);
            }
            else
            {
                config.URLS = null;
            }

            // set websitesOn
            if (Request["websitesOn"] == "on")
            {
                config.URLActive = true;
            }
            else
            {
                config.URLActive = false;
            }

            //save to db
            cc.UpdateEntity(config);

            return this.Redirect("/System/SearchConfig?sysguid=" + systemguid);
        }


        [Authorize]
        [HttpPost]
        public ActionResult LoadConfig()
        {
            // get data from GET-Request
            string systemguid = Request.QueryString["sysguid"];

            // get data from GET-Request
            string loadedConfigId = Request["orgUnitConfigId"];

            // init
            var cc = WCFControllerClient<IDBManager>.ClientProxy;

            //get config from other orgUnit
            var loadedConfig = cc.GetEntity(new Guid(loadedConfigId), cc.LoadThisEntities("OrgUnitConfig")) as OrgUnitConfig;

            // get SystemConfig, OrgUnitConfig
            var orgUnit = cc.GetEntity(new Guid(systemguid), cc.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
            var config = orgUnit.OrgUnitConfig;

            // copy orgUnitConfig data 
            config.EmailActive = loadedConfig.EmailActive;
            config.URLActive = loadedConfig.URLActive;
            config.SchedulerActive = loadedConfig.SchedulerActive;
            config.Days = loadedConfig.Days;
            config.Time = loadedConfig.Time;
            config.Emails = loadedConfig.Emails;
            config.URLS = loadedConfig.URLS;
            
            cc.UpdateEntity(config);


            return this.Redirect("/System/SearchConfig?sysguid=" + systemguid);
        }
    }
}

