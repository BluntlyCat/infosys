// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.DBManager.Data;
    using HSA.InfoSys.Logging;
    using HSA.InfoSys.SolrClient;
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
        private static readonly ILog Log = Logging.GetLogger("CrawlController");

        /// <summary>
        /// The db manager.
        /// </summary>
        private static IDBManager dbManager = DBManager.GetDBManager();

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

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

        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public Guid AddEntity(Entity entity)
        {
            return dbManager.AddEntity(entity);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public Guid UpdateEntity(Entity entity)
        {
            return dbManager.UpdateEntity(entity);
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <typeparam name="T">The type of what you want.</typeparam>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public Entity GetEntity(Guid entityGuid)
        {
            return dbManager.GetEntity<Entity>(entityGuid);
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The new component.
        /// </returns>
        public Component CreateComponent(string name, string category)
        {
            return dbManager.CreateComponent(name, category);
        }

        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="sourceURL">The source URL.</param>
        /// <returns>
        /// The new source.
        /// </returns>
        public Source CreateSource(string sourceURL)
        {
            return dbManager.CreateSource(sourceURL);
        }

        #endregion
    }
}
