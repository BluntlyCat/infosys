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
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using WCFServices;

    /// <summary>
    ///  SolrClient deals as an API
    /// </summary>
    public class SolrSearchController
    {
        /// <summary>
        /// The logger for SolrClient.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrSearchController");

        /// <summary>
        /// The db mutex.
        /// </summary>
        private static readonly Mutex DbMutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// The components finished.
        /// </summary>
        private int componentsFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrSearchController"/> class.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        public SolrSearchController(IDbManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="settings">The settings for SolrSearchClient.</param>
        /// <param name="query">The query.</param>
        /// <param name="componentGUID">The component GUID.</param>
        private delegate void InvokeSolrSearch(SolrSearchClientSettings settings, string query, Guid componentGUID);

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
            var settings = this.dbManager.GetSolrClientSettings();

            if (settings.Equals(new SolrSearchClientSettings()) == false)
            {
                var orgUnit = this.dbManager.GetEntity(orgUnitGuid, this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                if (orgUnit != null)
                {
                    this.OrgUnitGuid = orgUnitGuid;

                    var components = this.dbManager.GetComponentsByOrgUnitId(this.OrgUnitGuid).ToList();

                    foreach (var component in components)
                    {
                        var searchClient = new SolrSearchClient(this.dbManager);

                        var results = DbManager.Session.QueryOver<Result>()
                            .Where(c => component != null && c.ComponentGUID == component.EntityId)
                            .List();

                        var resultPot = new SolrResultPot();
                        var sendResults = new List<Result>();

                        ////Here we tell our delegate which method to call.
                        var invokeSearch = new InvokeSolrSearch(searchClient.StartSearch);

                        ////This is our callback method which will be
                        ////called when solr finished the searchrequest.
                        var callback = new AsyncCallback(
                            c =>
                                {
                                    DbMutex.WaitOne();

                                    if (c.IsCompleted)
                                    {
                                        try
                                        {
                                            componentsFinished++;

                                            resultPot = searchClient.GetResult();

                                            if (resultPot.HasResults)
                                            {
                                                sendResults = this.GetSendResults(resultPot, results);

                                                var comp = dbManager.GetEntity(resultPot.EntityId) as Component;

                                                if (comp != null)
                                                {
                                                    Log.InfoFormat(
                                                        Properties.Resources.SOLR_SEARCH_COMPONENT_FINISHED,
                                                        comp.Name);
                                                }
                                            }
                                            else
                                            {
                                                Log.WarnFormat(
                                                    Properties.Resources.SOLR_SEARCH_NO_RESULTS,
                                                    orgUnit.Name);
                                            }
                                        }
                                        catch (SolrResponseBadRequestException bre)
                                        {
                                            Log.ErrorFormat(
                                                Properties.Resources.SOLR_SEARCH_CONTROLLER_RESULT_ERROR,
                                                bre);
                                        }
                                        catch (Exception e)
                                        {
                                            Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                                        }
                                    }

                                    if (this.componentsFinished == components.Count)
                                    {
                                        this.SendResults(sendResults, orgUnit);
                                    }

                                    DbMutex.ReleaseMutex();
                                });

                        invokeSearch.BeginInvoke(settings, component.Name, component.EntityId, callback, this);
                        Log.InfoFormat(Properties.Resources.SOLR_SEARCH_COMPONENT_STARTED, component.Name);
                    }
                }
                else
                {
                    //// No OrgUnit
                    Log.WarnFormat(Properties.Resources.SOLR_SEARCH_CONTROLLER_NO_ORGUNIT, orgUnitGuid);
                }
            }
            else
            {
                //// No settings
                Log.WarnFormat(Properties.Resources.SOLR_SEARCH_CONTROLLER_NO_SETTINGS);
            }
        }

        /// <summary>
        /// Gets the send results.
        /// </summary>
        /// <param name="resultPot">The result pot.</param>
        /// <param name="results">The results.</param>
        /// <returns>A list of the results to store in database.</returns>
        private List<Result> GetSendResults(SolrResultPot resultPot, ICollection<Result> results)
        {
            var sendResults = new List<Result>();

            foreach (var result in resultPot.Results)
            {
                if (results.Count == 0 ||
                    !results.Any(r => r.Content.Equals(result.Content)))
                {
                    sendResults.Add(result);
                    result.ComponentGUID = resultPot.EntityId;
                    this.dbManager.AddEntity(result);
                }

                Log.InfoFormat(
                    Properties.Resources.SOLR_SEARCH_RESULT,
                    result.ComponentGUID,
                    result);
            }

            return sendResults;
        }

        /// <summary>
        /// Sends the results.
        /// </summary>
        /// <param name="sendResults">The send results.</param>
        /// <param name="orgUnit">The org unit.</param>
        private void SendResults(List<Result> sendResults, OrgUnit orgUnit)
        {
            try
            {
                if (sendResults.Count > 0 && orgUnit.OrgUnitConfig.EmailActive)
                {
                    var mailNotifier = new EmailNotifier(this.dbManager);
                    mailNotifier.SearchFinished(this.OrgUnitGuid, sendResults);
                }
                else if (sendResults.Count == 0)
                {
                    Log.InfoFormat(
                        Properties.Resources.SOLR_SEARCH_CONTROLLER_NO_NEW_RESULTS,
                        orgUnit.Name);
                }

                Log.InfoFormat(
                    Properties.Resources.SOLR_SEARCH_ORGUNIT_FINISHED,
                    orgUnit.Name);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }
    }
}