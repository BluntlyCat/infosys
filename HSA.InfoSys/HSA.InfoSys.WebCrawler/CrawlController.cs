// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.DBManager.Data;
    using HSA.InfoSys.Logging;
    using log4net;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the gui by using wcf.
    /// </summary>
    public class CrawlController : ICrawlController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static ILog log = Logging.GetLogger("CrawlController");

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// The db manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// Tests this instance.
        /// </summary>
#warning Only for testing, remove me when finished.
        public void Test()
        {
            // Beispiel
            string s = "99b7816e-4ca1-49a7-a607-a1bc00ff72a5";

            var newComp = this.dbManager.CreateComponent("Windows8", "TestWin");
            this.dbManager.AddNewObject(newComp);
            var existingComp = this.dbManager.GetEntity<Component>(new Guid(s));
            SolrClient c = new SolrClient(8983, "141.82.59.193");
            c.SolrQuery("solr", SolrOutputMimeType.xml);
        }

        /// <summary>
        /// Opens the WCF host.
        /// </summary>
        public void OpenWCFHost()
        {
            log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_OPENED);
            this.host.Open();
        }

        /// <summary>
        /// Closes the WCF host.
        /// </summary>
        public void CloseWCFHost()
        {
            log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_CLOSED);
            this.host.Close();
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        public void StartSearch()
        {
            log.Info(Properties.Resources.CRAWL_CONTROLLER_SEARCH_STARTED);
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            this.host = new ServiceHost(typeof(CrawlController));
            this.dbManager = DBManager.GetDBManager();
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public void StopServices()
        {
            log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);
        }
    }
}
