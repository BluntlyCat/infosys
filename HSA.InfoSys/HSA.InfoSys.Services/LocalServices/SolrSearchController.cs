// ------------------------------------------------------------------------
// <copyright file="SolrSearchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Linq;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    /// <summary>
    ///  SolrClient deals as an API
    /// </summary>
    public class SolrSearchController
    {
        /// <summary>
        /// The logger for SolrClient.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrClient");

        /// <summary>
        /// The db mutex.
        /// </summary>
        private static Mutex dbMutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager = DBManager.ManagerFactory;

        /// <summary>
        /// The components finished.
        /// </summary>
        private int componentsFinished = 0;

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="componentGUID">The component GUID.</param>
        public delegate void InvokeSolrSearch(string query, Guid componentGUID);

        /// <summary>
        /// Gets or sets the org unit GUID.
        /// </summary>
        /// <value>
        /// The org unit GUID.
        /// </value>
        private Guid OrgUnitGuid { get; set; }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        public void StartSearch(Guid orgUnitGuid)
        {
            this.OrgUnitGuid = orgUnitGuid;

            var components = this.dbManager.GetComponentsByOrgUnitId(this.OrgUnitGuid).ToList<Component>();

            if (components == null)
            {
                WCFControllerClient<ISearchRecall>.ClientProxy.Recall(this.OrgUnitGuid);
                return;
            }

            foreach (var component in components)
            {
                Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SEARCH_STARTED, component.Name);

                var searchClient = new SolrSearchClient();

                ////Here we tell our delegate which method to call.
                InvokeSolrSearch invokeSearch = new InvokeSolrSearch(searchClient.StartSearch);

                ////This is our callback method which will be
                ////called when solr finished the searchrequest.
                AsyncCallback callback = new AsyncCallback(
                    c =>
                    {
                        dbMutex.WaitOne();

                        if (c.IsCompleted)
                        {
                            var resultPot = searchClient.GetResult();

                            foreach (var result in resultPot.Results)
                            {
                                result.Component = dbManager.GetEntity(resultPot.EntityId) as Component;

                                dbManager.AddEntity(result);

                                Log.InfoFormat(
                                    Properties.Resources.QUERY_RESPONSE,
                                    result.Component.Name,
                                    result);
                            }

                            componentsFinished++;
                        }

                        if (this.componentsFinished == components.Length)
                        {
                            WCFControllerClient<ISearchRecall>.ClientProxy.Recall(this.OrgUnitGuid);
                        }

                        dbMutex.ReleaseMutex();
                    });

                invokeSearch.BeginInvoke(component.Name, component.EntityId, callback, this);
            }
        }

        /// <summary>
        /// Threads the routine.
        /// </summary>
        /// <param name="solrQuery">The solr query.</param>
        /// <returns>The search result from solr.</returns>
        /*private void ThreadRoutine(string solrQuery)
        {
            string response = string.Empty;

            // Main Loop which is checking, whether there is an message for the server or not
            while (this.Running && this.SolrSocket.Connected)
            {
                // waiting for the server's responde
                response = this.InvokeSolrQuery(solrQuery);

                Thread.Sleep(100);
            }

            this.SolrResponse = response;
        }*/
    }
}