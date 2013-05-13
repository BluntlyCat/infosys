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
            SolrClient client = new SolrClient(8983, "141.82.59.139");
            client.SolrQuery("solr", SolrOutputMimeType.xml);
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
