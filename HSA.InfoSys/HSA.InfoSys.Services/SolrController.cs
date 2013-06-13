// ------------------------------------------------------------------------
// <copyright file="SolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.SolrClient;
    using log4net;

    /// <summary>
    /// In this class are all methods implemented for controlling Solr.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SolrController : Service, ISolrController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlController");

        /// <summary>
        /// The solr controller.
        /// </summary>
        private static SolrController solrController;

        /// <summary>
        /// The db mutex.
        /// </summary>
        private static Mutex dbMutex = new Mutex();

        /// <summary>
        /// The data base manager.
        /// </summary>
        private IDBManager dbManager = DBManager.ManagerFactory;

        /// <summary>
        /// Prevents a default instance of the <see cref="SolrController"/> class from being created.
        /// </summary>
        private SolrController()
        {
        }

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="componentGUID">The component GUID.</param>
        public delegate void InvokeSolrSearch(string query, Guid componentGUID);

        /// <summary>
        /// Gets the solr controller.
        /// </summary>
        /// <value>
        /// The solr controller.
        /// </value>
        public static SolrController SolrFactory
        {
            get
            {
                if (solrController == null)
                {
                    solrController = new SolrController();
                }

                return solrController;
            }
        }

        /// <summary>
        /// Searches for all components of an org unit.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void SearchForOrgUnit(Guid orgUnitGUID)
        {
            var components = this.dbManager.GetComponentsByOrgUnitId(orgUnitGUID);
            this.Search(components);
        }

        /// <summary>
        /// Searches for one component.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        public void SearchForComponent(Guid componentGUID)
        {
            var list = new List<Component>();
            var component = this.dbManager.GetEntity(componentGUID) as Component;

            list.Add(component);

            this.Search(list);
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="components">The components.</param>
        public void Search(IList<Component> components)
        {
            foreach (var component in components)
            {
                Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SEARCH_STARTED, component.Name);

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
                        dbMutex.WaitOne();

                        if (c.IsCompleted)
                        {
                            var resultPot = client.GetResult();

                            foreach (var result in resultPot.Results)
                            {
                                result.Component = dbManager.GetEntity(resultPot.EntityId) as Component;

                                dbManager.AddEntity(result);

                                Log.InfoFormat(
                                    Properties.Resources.QUERY_RESPONSE,
                                    result.Component.Name,
                                    result);
                            }
                        }

                        dbMutex.ReleaseMutex();
                    });

                invokeSearch.BeginInvoke(component.Name, component.EntityId, callback, this);
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }
    }
}
