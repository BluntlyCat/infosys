// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System.ServiceModel;
    using System.Linq;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.Logging;
    using HSA.InfoSys.SolrClient;
    using log4net;
    using System;
    using System.Collections.Generic;

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
        private static ILog Log = Logging.GetLogger("CrawlController");

        public delegate void InvokeSolrSearch();

        private static InvokeSolrSearch invokeSearch;

        private static SolrClient client = new SolrClient(
                Properties.Settings.Default.SOLR_PORT,
                Properties.Settings.Default.SOLR_HOST);

        private static List<Guid> searchTickets = new List<Guid>();

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
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_OPENED);
            this.host.Open();
        }

        /// <summary>
        /// Closes the WCF host.
        /// </summary>
        public void CloseWCFHost()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_CLOSED);
            this.host.Close();
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        public int StartSearch(string query)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SEARCH_STARTED);

            searchTickets.Add(client.SolrQuery(query, SolrOutputMimeType.xml));
            
            invokeSearch = new InvokeSolrSearch(client.StartSearch);
            AsyncCallback callback = new AsyncCallback(SearchFinished);

            invokeSearch.BeginInvoke(callback, this);

            return 0;
        }

        /// <summary>
        /// Gets the response from solr.
        /// </summary>
        /// <param name="key">The response key.</param>
        /// <returns>
        /// The response by key.
        /// </returns>
        public static void SearchFinished(IAsyncResult result)
        {
            Log.Info("AsyncCallback invoked");

            foreach (var ticket in searchTickets)
            {
                Log.InfoFormat("Response for ticket [{0}] is [{1}]", ticket, client.GetRespondByTicket(ticket));
            }

            searchTickets.Clear();
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
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);
        }
    }
}
