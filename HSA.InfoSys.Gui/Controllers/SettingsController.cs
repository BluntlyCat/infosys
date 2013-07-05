// ------------------------------------------------------------------------
// <copyright file="SettingsController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.ServiceModel;
    using System.Text;
    using System.Web.Mvc;
    using Common.Entities;
    using Common.Logging;
    using Common.Services.LocalServices;
    using Common.Services.WCFServices;
    using Newtonsoft.Json;
    using log4net;

    /// <summary>
    /// The controller for the home page.
    /// </summary>
    [HandleError]
    public class SettingsController : Controller
    {
        /// <summary>
        /// The logger for the home controller
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SettingsController");

        /// <summary>
        /// The settings for WCF.
        /// </summary>
        private static readonly WCFSettings Settings = new WCFSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        public SettingsController()
        {
            Settings.InitializeSettings("localhost", 8085, "CrawlerProxy", "localhost", 8086, "CrawlerProxy");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                this.ViewData["navid"] = "serversettings";
                this.ViewData["label1"] = Properties.Resources.TEST_LABLE1;

                // get all Settings from DB
                this.ViewData["MailSettings"] = cc.GetMailSettings();
                this.ViewData["NutchClientSettings"] = cc.GetNutchClientSettings();
                this.ViewData["SolrClientSettings"] = cc.GetSolrClientSettings();
                this.ViewData["WCFSettings"] = cc.GetWcfSettings();
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }

            return this.View();
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Email()
        {
            try
            {
                this.ViewData["navid"] = "serversettings";
                //// get POST data from form
                string smtpserver = Request["smtpserver"];
                string smtpport = Request["smtpport"];
                string mailfrom = Request["mailfrom"];

                //// @TODO check for null values

                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                var mailSettings = cc.GetMailSettings();

                mailSettings.SmtpServer = smtpserver;
                mailSettings.SmtpPort = int.Parse(smtpport.Replace(" ", ""));
                mailSettings.MailFrom = mailfrom;

                cc.UpdateEntity(mailSettings);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }

            return this.RedirectToAction("Index", "Settings");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Nutch()
        {
            try
            {
                this.ViewData["navid"] = "serversettings";

                // get POST data from form
                string homepath = Request["homepath"];
                string nutchpath = Request["nutchpath"];
                string nutchcommand = Request["nutchcommand"];
                string nutchclients = Request["nutchclients"];
                string defaulturls = Request["defaulturls"];
                string crawldepth = Request["crawldepth"];
                string crawltopn = Request["crawltopn"];
                string solrserver = Request["solrserver"];
                string javahome = Request["javahome"];
                string certificatepath = Request["certificatepath"];
                string prefix = Request["prefix"];

                //// @TODO check for null values

                //// get settings from db
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                var nutchClientSettings = cc.GetNutchClientSettings();

                //// change to new values
                nutchClientSettings.HomePath = homepath;
                nutchClientSettings.NutchPath = nutchpath;
                nutchClientSettings.NutchCommand = nutchcommand.Replace(" ", "");
                nutchClientSettings.NutchClients = JsonConvert.SerializeObject(nutchclients.Replace(" ", "").Split(','));
                nutchClientSettings.DefaultURLs = JsonConvert.SerializeObject(defaulturls.Replace(" ", "").Split(','));
                nutchClientSettings.CrawlDepth = int.Parse(crawldepth.Replace(" ", ""));
                nutchClientSettings.CrawlTopN = int.Parse(crawltopn.Replace(" ", ""));
                nutchClientSettings.SolrServer = solrserver.Replace(" ", "");
                nutchClientSettings.JavaHome = javahome;
                nutchClientSettings.CertificatePath = certificatepath;
                nutchClientSettings.Prefix = prefix;

                //// save into db
                cc.UpdateEntity(nutchClientSettings);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }

            return this.RedirectToAction("Index", "Settings");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Solr()
        {
            try
            {
                this.ViewData["navid"] = "serversettings";

                // get POST data from form
                string host = Request["host"];
                string port = Request["port"];
                string collection = Request["collection"];
                string filterquery = Request["filterquery"];

                //// @TODO check for null values

                //// get settings from db
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                var solrClientSettings = cc.GetSolrClientSettings();

                //// save changes
                solrClientSettings.Host = host;
                solrClientSettings.Port = int.Parse(port);
                solrClientSettings.Collection = collection;
                solrClientSettings.FilterQuery = filterquery;

                //// save to db
                cc.UpdateEntity(solrClientSettings);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }

            return this.RedirectToAction("Index", "Settings");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult WcfSettings()
        {
            try
            {
                this.ViewData["navid"] = "serversettings";

                //// get POST data from form
                string httphost = Request["httphost"];
                string httpport = Request["httpport"];
                string httppath = Request["httphost"];

                string nettcphost = Request["nettcphost"];
                string nettcpport = Request["nettcpport"];
                string nettcppath = Request["nettcppath"];

                string certificatepath = Request["certificatepath"];
                string certificatepassword = Request["certificatepassword"];

                //// @TODO check for null values

                //// get settings from db
                var cc = WCFControllerClient<IDbManager>.GetClientProxy(Settings);
                var wcfSettings = cc.GetWcfSettings();

                //// save changes
                wcfSettings.HttpHost = httphost;
                wcfSettings.HttpPort = int.Parse(httpport.Replace(" ", ""));
                wcfSettings.HttpPath = httppath;

                wcfSettings.NetTcpHost = nettcphost;
                wcfSettings.NetTcpPort = int.Parse(nettcpport.Replace(" ", ""));
                wcfSettings.NetTcpPath = nettcppath;

                wcfSettings.CertificatePath = certificatepath;
                wcfSettings.CertificatePassword = Encryption.Encrypt(Encoding.UTF8.GetBytes(certificatepassword));

                //// save into db
                cc.UpdateEntity(wcfSettings);
            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                //return to error page
                this.ViewData["error"] = "Error, please try again!";
                return this.View();
            }

            return this.RedirectToAction("Index", "Settings");
        }
    }
}
