// ------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
#if MONO
    using System.Collections.Generic;
#endif
    using System.Linq;
    using System.ServiceModel;
    using System.Web.Mvc;
    using System.Web.Security;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using Newtonsoft.Json;

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
        /// The settings for WCF.
        /// </summary>
        private static readonly WCFSettings Settings = new WCFSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemController"/> class.
        /// </summary>
        public SystemController()
        {
            Settings.InitializeSettings("localhost", 8085, "CrawlerProxy", "localhost", 8086, "CrawlerProxy");
        }

        /// <summary>
        /// Called when the home page is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get id of current logged-in user
                MembershipUser membershipuser = Membership.GetUser();

                string userid = membershipuser.ProviderUserKey.ToString();
                int uid = int.Parse(userid);
                Log.DebugFormat("Got user id {0}", uid);

                var orgUnits = cc.GetOrgUnitsByUserId(uid, cc.LoadThisEntities("OrgUnitConfig")).ToList<OrgUnit>();

                Log.DebugFormat("Got org units {0}", orgUnits);
                this.ViewData["orgUnits"] = orgUnits;
                this.ViewData["navid"] = "mysystems";


                // if search was started submit success 
                if (Request.QueryString["searchStarted"] == "true")
                {
                    this.ViewData["searchStarted"] = "true";
                }
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.View();
        }

        /// <summary>
        /// Is Called when a new OrgUnit was created
        /// </summary>
        /// <returns>Returns to the defined action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult IndexSubmit()
        {
            try
            {
                // get name of the new system
                string orgUnitName = Request["newsystem"];

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // log
                Log.Info("add new system");

                // get id of current logged-in user
                MembershipUser membershipuser = Membership.GetUser();
                string userid = membershipuser.ProviderUserKey.ToString();
                int id = Convert.ToInt32(userid);

                var mails = JsonConvert.SerializeObject(new string[] { membershipuser.Email });

                // TODO: default urls nicht hardcoded, sondern müssen hier noch zuvor aus der DB aus Settings ausgelesen werden
                var urls = JsonConvert.SerializeObject(new string[] { "http://www.heise.de/security/", "http://nvd.nist.gov/" });

                // create SystemConfig
                var orgUnitConfig = cc.CreateOrgUnitConfig(urls, mails, true, true, 1, 12, new DateTime(), true);

                // create System
                var orgUnit = cc.CreateOrgUnit(id, orgUnitName);
                orgUnit.OrgUnitConfig = orgUnitConfig;

                // save to db
                cc.AddEntity(orgUnit);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// Deletes the org unit.
        /// </summary>
        /// <returns>Returns to the defined action.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteOrgUnit()
        {
            try
            {
                // get systemguid from GET-Request
                Guid orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get orgUnit
                var orgUnit = cc.GetEntity(orgUnitGUID, cc.LoadThisEntities("OrgUnit", "OrgUnitConfig")) as OrgUnit;

                // Remove from scheduler if registered and stop.
                WCFControllerClient<IScheduler>.GetClientProxy(Settings).RemoveOrgUnitConfig(orgUnit.OrgUnitConfig.EntityId);

                // get all components by OrgUnitId
                var components = cc.GetComponentsByOrgUnitId(orgUnitGUID).ToList<Component>();

                // entities must be deleted in this order because of db dependencies
                // delete all components and their results 
                foreach (Component comp in components)
                {
                    cc.DeleteEntity(comp);
                }

                // delete OrgUnit and orgUnitConfig
                cc.DeleteEntity(orgUnit);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.RedirectToAction("Index", "System");
        }

        /// <summary>
        /// starts the real time search
        /// </summary>
        /// <returns>Returns to the defined action.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult RealTimeSearch()
        {
            // trigger search
            try
            {
                Guid orgUnitGuid = Guid.Parse(Request.QueryString["sysguid"]);
                WCFControllerClient<ISolrController>.GetClientProxy(Settings).SearchForOrgUnit(orgUnitGuid);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            //return this.RedirectToAction("Index", "System");
            return this.Redirect("/System/Index?searchStarted=true");
        }

        /// <summary>
        /// Called when page components is loading.
        /// </summary>
        /// <returns>Returns to this view.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult Components()
        {
            try
            {
                // get systemguid from GET-Request
                Guid orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get OrgUnit
                var orgUnit = cc.GetEntity(orgUnitGUID, cc.LoadThisEntities("OrgUnit", "OrgUnitConfig")) as OrgUnit;

                // get all components by OrgUnitId
                var components = cc.GetComponentsByOrgUnitId(orgUnitGUID).ToList<Component>();

                this.ViewData["navid"] = "mysystems";
                this.ViewData["systemguid"] = orgUnitGUID;
                this.ViewData["orgUnitName"] = orgUnit.Name;

                if (components.Count > 0)
                {
                    this.ViewData["components"] = components;
                }
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.View();
        }

        /// <summary>
        /// Submits the component.
        /// </summary>
        /// <returns>Redirect to the defined URL.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult SubmitComponent()
        {
            // get systemguid from GET-Request
            Guid orgUnitGUID = Guid.Empty;

            try
            {
                orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // get POST data from form
                string component = Request["components"];

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get orgUnit by id
                var orgUnit = cc.GetEntity(orgUnitGUID, cc.LoadThisEntities("OrgUnit", "OrgUnitConfig")) as OrgUnit;

                // log
                Log.Info("add new component");

                // save component to DB
                cc.AddEntity(cc.CreateComponent(component, orgUnit.EntityId));
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.Redirect("/System/Components?sysguid=" + orgUnitGUID);
        }

        /// <summary>
        /// Deletes the component.
        /// </summary>
        /// <returns>Redirect to the defined URL.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult DeleteComponent()
        {
            Guid orgUnitGUID = Guid.Empty;

            try
            {
                // get systemguid from GET-Request
                orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // get compid from GET-Request
                Guid componentGUID = Guid.Parse(Request.QueryString["compid"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get component by id
                var component = cc.GetEntity(componentGUID);

                // delete component
                cc.DeleteEntity(component);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.Redirect("/System/Components?sysguid=" + orgUnitGUID);
        }

        /// <summary>
        /// Called when page search is loading.
        /// </summary>
        /// <returns>
        /// Returns to this view.
        /// </returns>
        [Authorize]
        [HttpGet]
        public ActionResult SearchConfig()
        {
            try
            {
                // get systemguid from GET-Request
                Guid orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                var settings = WCFControllerClient<IDbManager>.GetClientProxy(Settings).GetNutchClientSettings();

                string defaulturls = settings.DefaultURLs;

                // default urls
                this.ViewData["defaulturls"] = defaulturls;

                // get id of current logged-in user
                MembershipUser membershipuser = Membership.GetUser();
                string userid = membershipuser.ProviderUserKey.ToString();
                int id = Convert.ToInt32(userid);

                var orgUnits = cc.GetOrgUnitsByUserId(id, cc.LoadThisEntities("OrgUnitConfig")).ToList<OrgUnit>();

                var delItem = orgUnits.Find(x => x.EntityId == orgUnitGUID);

                orgUnits.Remove(delItem);
                this.ViewData["orgUnits"] = orgUnits;

                // get SystemConfig, OrgUnitConfig
                var config = delItem.OrgUnitConfig;

                // set all config data for view
                this.ViewData["orgUnitName"] = delItem.Name;

                this.ViewData["schedulerActive"] = config.SchedulerActive;
                this.ViewData["emailActive"] = config.EmailActive;
                this.ViewData["urlActive"] = config.URLActive;

                this.ViewData["sc_days"] = config.Days;
                this.ViewData["sc_hours"] = config.Time;

                if (!string.IsNullOrEmpty(config.Emails))
                {
                    this.ViewData["emails"] = JsonConvert.DeserializeObject<string[]>(config.Emails);
                }
                else
                {
                    this.ViewData["emails"] = new string[0];
                }

                if (!string.IsNullOrEmpty(config.URLs))
                {
                    this.ViewData["urls"] = JsonConvert.DeserializeObject<string[]>(config.URLs);
                }
                else
                {
                    this.ViewData["urls"] = new string[0];
                }

                //// MembershipUser user = Membership.GetUser();
                //// this.ViewData["useremail"] = user.Email;

                this.ViewData["navid"] = "mysystems";
                this.ViewData["systemguid"] = orgUnitGUID;
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

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
            Guid orgUnitGUID = Guid.Empty;

            try
            {
                // get data from GET-Request
                orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get SystemConfig, OrgUnitConfig
                var orgUnit = cc.GetEntity(orgUnitGUID, cc.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
                var config = orgUnit.OrgUnitConfig;

                string sc_days = Request["sc_days"];
                string sc_time = Request["sc_time"];

                config.Days = Convert.ToInt32(sc_days);
                config.Time = Convert.ToInt32(sc_time);

                // set schedulerOn, days, time
                if (this.Request["schedulerOn"] == "on")
                {
                    config.SchedulerActive = true;
                    WCFControllerClient<IScheduler>.GetClientProxy(Settings).AddOrgUnitConfig(config);
                }
                else
                {
                    config.SchedulerActive = false;
                    WCFControllerClient<IScheduler>.GetClientProxy(Settings).RemoveOrgUnitConfig(config.EntityId);
                }

                // set Emails
                if (!string.IsNullOrEmpty(this.Request["emails[]"]))
                {
                    string[] emails = Request["emails[]"].Split(',');
                    config.Emails = JsonConvert.SerializeObject(emails);
                }
                else
                {
                    config.Emails = null;
                }

                config.EmailActive = this.Request["emailsOn"] == "on";

                // set Websites
                if (!string.IsNullOrEmpty(this.Request["websites[]"]))
                {
                    string[] websites = Request["websites[]"].Split(',');
                    config.URLs = JsonConvert.SerializeObject(websites);
                }
                else
                {
                    config.URLs = null;
                }

                // save to db
                cc.UpdateEntity(config);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.Redirect("/System/SearchConfig?sysguid=" + orgUnitGUID);
        }

        /// <summary>
        /// Loads the config.
        /// </summary>
        /// <returns>Redirect to the defined URL.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult LoadConfig()
        {
            // get data from GET-Request
            Guid orgUnitGUID = Guid.Empty;

            try
            {
                orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                // get data from GET-Request
                Guid loadedConfigId = Guid.Parse(Request["orgUnitConfigId"]);

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get config from other orgUnit
                var loadedConfig = cc.GetEntity(loadedConfigId) as OrgUnitConfig;

                // get SystemConfig, OrgUnitConfig
                var orgUnit = cc.GetEntity(orgUnitGUID, cc.LoadThisEntities("OrgUnitConfig")) as OrgUnit;
                var config = orgUnit.OrgUnitConfig;

                // copy orgUnitConfig data 
                config.EmailActive = loadedConfig.EmailActive;
                config.URLActive = loadedConfig.URLActive;
                config.SchedulerActive = loadedConfig.SchedulerActive;
                config.Days = loadedConfig.Days;
                config.Time = loadedConfig.Time;
                config.Emails = loadedConfig.Emails;
                config.URLs = loadedConfig.URLs;

                cc.UpdateEntity(config);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.Redirect("/System/SearchConfig?sysguid=" + orgUnitGUID);
        }

        /// <summary>
        /// Shows the results.
        /// </summary>
        /// <returns>Returns to this view.</returns>
        [Authorize]
        [HttpGet]
        public ActionResult ShowResults()
        {
            try
            {
                // get data from GET-Request
                var orgUnitGUID = Guid.Parse(Request.QueryString["sysguid"]);

                var componentGUID = Request.QueryString["compguid"];

                // init
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

                // get all components by OrgUnitId
                var components = cc.GetComponentsByOrgUnitId(orgUnitGUID).ToList<Component>();

                if (components.Count != 0)
                {
                    // if a compGUID was selected by the user then show the results 
                    // else show the results of the first component
                    if (componentGUID != null)
                    {
                        var selectedCompGUID = Guid.Parse(componentGUID);
                        var component = cc.GetEntity(selectedCompGUID) as Component;

#if MONO
                        var results = GetResults(selectedCompGUID);
#else
                        var results = cc.GetResultsByComponentId(selectedCompGUID).ToList<Result>();
#endif
                        this.ViewData["selectedComp"] = component.Name;
                        this.ViewData["results"] = results;
                    }
                    else
                    {
                        var selectedComp = components.First();

#if MONO
                        var results = GetResults(selectedComp.EntityId);
#else
                        var results = cc.GetResultsByComponentId(selectedComp.EntityId).ToList<Result>();
#endif

                        this.ViewData["selectedComp"] = selectedComp.Name;
                        this.ViewData["results"] = results;
                    }

                    this.ViewData["systemguid"] = orgUnitGUID;
                    this.ViewData["components"] = components;
                    this.ViewData["navid"] = "mysystems";
                }
                else
                {
                    return this.RedirectToAction("Index", "System");
                }
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = Properties.Resources.ERROR_REDIRECT_MESSAGE;
                return this.View();
            }

            return this.View();
        }

#if MONO
        /// <summary>
        /// Gets the results.
        /// Because of restrictions of maximum amount of bytes (2^16)
        /// which can be send in MONO over WCF we need this special
        /// implementation if we run our server under unix based
        /// operating systems. It fetches the results from the database
        /// manager behind witch splits the results in pieces of
        /// 2^15 bytes until all results are received.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns>A list of results which belongs to the given component.</returns>
        private static List<Result> GetResults(Guid componentGUID)
        {
            var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);

            var allResults = cc.GetResultsByComponentId(componentGUID);
            var indexes = cc.GetResultIndexes(componentGUID, allResults).ToArray();
            var splittedResults = new List<Result[]>();

            for (int i = 0; i < indexes.Length - 1; i++)
            {
                splittedResults.Add(cc.GetResultsByRequestIndex(indexes[i], indexes[i + 1], allResults));
            }

            return splittedResults.SelectMany(splittedResult => splittedResult).ToList();
        }
#endif
    }
}