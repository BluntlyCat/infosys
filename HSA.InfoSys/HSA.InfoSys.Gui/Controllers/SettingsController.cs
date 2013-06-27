// ------------------------------------------------------------------------
// <copyright file="SettingsController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.ServiceModel;
    using System.Web.Mvc;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;
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
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var cc = WCFControllerClient<IDBManager>.ClientProxy;
                this.ViewData["navid"] = "serversettings";
                this.ViewData["label1"] = Properties.Resources.TEST_LABLE1;
                
                // get all Settings from DB
                this.ViewData["MailSettings"] = cc.GetMailSettings();
                this.ViewData["NutchClientSettings"] = cc.GetNutchClientSettings();
                this.ViewData["SolrClientSettings"] = cc.GetSolrClientSettings();
                this.ViewData["WCFAddressesSettings"] = cc.GetWCFAddressesSettings();
                this.ViewData["WCFControllerSettings"] = cc.GetWCFControllerSettings();

            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
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
                // get POST data from form
                string smtpserver = Request["smtpserver"];
                string smtpport = Request["smtpport"];
                string mailfrom = Request["mailfrom"];

                //@TODO check for null values

                var cc = WCFControllerClient<IDBManager>.ClientProxy;
                var mailSettings = cc.GetMailSettings();

                mailSettings.SmtpPort = int.Parse(smtpport);
                mailSettings.SmtpServer = smtpserver;
                mailSettings.MailFrom = mailfrom;


                cc.UpdateEntity(mailSettings);

            }
            catch (CommunicationException ce)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMUNICATION_ERROR, ce);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
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
            this.ViewData["navid"] = "serversettings";

            // get POST data from form
            string solrserver = Request["solrserver"];
            string seedfilename = Request["seedfilename"];
            string baseurlpath = Request["baseurlpath"];
            string nutchcommand = Request["nutchcommand"];
            string crawlrequest = Request["crawlrequest"];
            string basecrawlpath = Request["basecrawlpath"];
            string crawldepth = Request["crawldepth"];
            string crawltopn = Request["crawltopn"];
            string prefixpath = Request["prefixpath"];
            string prefixfilename = Request["prefixfilename"];
            string prefix = Request["prefix"];

            // TODO: save Post-Data in DB

            return this.RedirectToAction("Settings", "Index");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Solr()
        {
            this.ViewData["navid"] = "serversettings";

            // get POST data from form
            string host = Request["host"];
            string port = Request["port"];
            string collection = Request["collection"];
            string queryformat = Request["queryformat"];
            string requestformat = Request["requestformat"];
            string filterqueryformat = Request["filterqueryformat"];
            string filter = Request["filter"];

            // TODO: save Post-Data in DB

            return this.RedirectToAction("Settings", "Index");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult WcfAddresses()
        {
            this.ViewData["navid"] = "serversettings";

            // get POST data from form
            string httpaddress = Request["httpaddress"];
            string nettcpaddress = Request["nettcpaddress"];
            string httpport = Request["httpport"];
            string nettcpport = Request["nettcpport"];

            // TODO: save Post-Data in DB

            return this.RedirectToAction("Settings", "Index");
        }

        /// <summary>
        /// Shows the home page.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult WcfHost()
        {
            this.ViewData["navid"] = "serversettings";

            // get POST data from form
            string certificatepath = Request["certificatepath"];
            string certificatepassword = Request["certificatepassword"];

            // TODO: save Post-Data in DB

            return this.RedirectToAction("Settings", "Index");
        }
    }
}
