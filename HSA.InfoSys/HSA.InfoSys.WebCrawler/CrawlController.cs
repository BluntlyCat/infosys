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
    using HSA.InfoSys.Logging;
    using HSA.InfoSys.SolrClient;
    using log4net;
    using HSA.InfoSys.DBManager.Data;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the gui by using wcf.
    /// </summary>
    [KnownType(typeof(Component))]
    public class CrawlController : ICrawlController, IDBManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("CrawlController");

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// The db manager.
        /// </summary>
        private static IDBManager dbManager = DBManager.GetDBManager();

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="query">The query.</param>
        public delegate void InvokeSolrSearch(string query);

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

        #region ICrawlController Service Contract

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        public void StartSearch(string query)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SEARCH_STARTED);

            var client = new SolrClient(
                Properties.Settings.Default.SOLR_PORT,
                Properties.Settings.Default.SOLR_HOST);
            
            ////Here we tell our delegate which method to call.
            InvokeSolrSearch invokeSearch = new InvokeSolrSearch(client.StartSearch);

            ////This is our callback method which will be
            ////called when solr finished the searchrequest.
            AsyncCallback callback = new AsyncCallback(
                c =>
                {
                    if (c.IsCompleted)
                    {
                        Log.InfoFormat("Response for query [{0}] is\r\n[{1}]", query, client.GetResponse());
                    }
                });

            invokeSearch.BeginInvoke(query, callback, this);
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            this.host = new ServiceHost(typeof(CrawlController));
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public void StopServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);
        }

        #endregion

        #region IDBManager Service Contract

        public void AddEntity(object entity)
        {
            dbManager.AddEntity(entity);
        }

        public void UpdateEntity(object entity)
        {
            dbManager.UpdateEntity(entity);
        }

        public Component GetComponent(Guid componentGuid)
        {
            return dbManager.GetComponent(componentGuid);
        }

        public Source GetSource(Guid sourceGuid)
        {
            return dbManager.GetSource(sourceGuid);
        }

        public Component CreateComponent(string name, string categroy)
        {
            return dbManager.CreateComponent(name, categroy);
        }

        public Source CreateSource(string sourceURL)
        {
            return dbManager.CreateSource(sourceURL);
        }

        #endregion
    }
}
