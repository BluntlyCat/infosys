// ------------------------------------------------------------------------
// <copyright file="SolrSearchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
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
        private IDBManager dbManager = DBManager.ManagerFactory(Guid.NewGuid());

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
                return;
            }

            foreach (var component in components)
            {
                var searchClient = new SolrSearchClient();

                var results = DBManager.Session.QueryOver<Result>()
                    .Where(c => c.ComponentGUID == component.EntityId)
                    .List();

                SolrResultPot resultPot = new SolrResultPot(component.EntityId);

                ////Here we tell our delegate which method to call.
                InvokeSolrSearch invokeSearch = new InvokeSolrSearch(searchClient.StartSearch);

                ////This is our callback method which will be
                ////called when solr finished the searchrequest.
                AsyncCallback callback = new AsyncCallback(
                    c =>
                    {
                        dbMutex.WaitOne();
                        var sendResults = new List<Result>();
                        if (c.IsCompleted)
                        {
                            resultPot = searchClient.GetResult();

                            foreach (var result in resultPot.Results)
                            {
                                if (results.Count == 0 || !results.Any(r => r.Content.Equals(result.Content)))
                                {
                                    sendResults.Add(result);
                                    result.ComponentGUID = resultPot.EntityId;
                                    dbManager.AddEntity(result);
                                }

                                Log.InfoFormat(
                                    Properties.Resources.SOLR_SEARCH_RESULT,
                                    result.ComponentGUID,
                                    result);
                            }

                            componentsFinished++;

                            var comp = dbManager.GetEntity(resultPot.EntityId) as Component;
                            Log.InfoFormat(Properties.Resources.SOLR_SEARCH_COMPONENT_FINISHED, comp.Name);
                        }

                        if (this.componentsFinished == components.Count)
                        {
                            try
                            {
                                var orgUnit = dbManager.GetEntity(orgUnitGuid) as OrgUnit;

                                if (resultPot.HasResults)
                                {
                                    if (sendResults.Count > 0)
                                    {
                                        EmailNotifier mailNotifier = new EmailNotifier();
                                        mailNotifier.SearchFinished(this.OrgUnitGuid, sendResults);
                                    }
                                    else
                                    {
                                        Log.InfoFormat(Properties.Resources.SOLR_SEARCH_CONTROLLER_NO_NEW_RESULTS, orgUnit.Name);
                                    }
                                }
                                else
                                {
                                    Log.WarnFormat(Properties.Resources.SOLR_SEARCH_NO_RESULTS, orgUnit.Name);
                                }

                                Log.InfoFormat(Properties.Resources.SOLR_SEARCH_ORGUNIT_FINISHED, orgUnit.Name);
                            }
                            catch (Exception e)
                            {
                                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                            }
                        }

                        dbMutex.ReleaseMutex();
                    });

                invokeSearch.BeginInvoke(component.Name, component.EntityId, callback, this);

                Log.InfoFormat(Properties.Resources.SOLR_SEARCH_COMPONENT_STARTED, component.Name);
            }
        }
    }
}