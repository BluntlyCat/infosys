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
    /// The solr search controller handles a search request
    /// for one OrgUnit and instanciates a new client for each component.
    /// There will be one controller for each OrgUnit we want search.
    /// </summary>
    public class SolrSearchController
    {
        /// <summary>
        /// The logger for SolrSearchController.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrSearchController");

        /// <summary>
        /// The db mutex.
        /// </summary>
        private readonly Mutex dbMutex = new Mutex();

        /// <summary>
        /// The database dbmanager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// The components finished.
        /// Indicates if there is still a search running
        /// for a component. Will have the amount of components
        /// when finished.
        /// </summary>
        private int componentsFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrSearchController"/> class.
        /// </summary>
        /// <param name="dbManager">The db dbmanager.</param>
        public SolrSearchController(IDbManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="settings">The settings for SolrSearchClient.</param>
        private delegate void InvokeSolrSearch(SolrSearchClientSettings settings);

        /// <summary>
        /// Gets or sets the org unit GUID.
        /// </summary>
        /// <value>
        /// The org unit GUID.
        /// </value>
        private Guid OrgUnitGuid { get; set; }

        /// <summary>
        /// Starts the search on each client but first updates the settings
        /// and fetches all components of the OrgUnit for which this controller
        /// is responsible. This method also contains the callback method
        /// the clients calls when finished.
        /// </summary>
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        public void StartSearch(Guid orgUnitGuid)
        {
            var settings = this.dbManager.GetSolrClientSettings();

            if (settings.IsDefault() == false)
            {
                var orgUnit = this.dbManager.GetEntity(
                    orgUnitGuid,
                    this.dbManager.LoadThisEntities("OrgUnitConfig")) as OrgUnit;

                if (orgUnit != null)
                {
                    this.OrgUnitGuid = orgUnitGuid;

                    var components = this.dbManager.GetComponentsByOrgUnitId(this.OrgUnitGuid).ToList();
                    var sendResults = new List<Result>();

                    foreach (var component in components)
                    {
                        var searchClient = new SolrSearchClient(this.dbManager, component.EntityId, component.Name);

                        var innerComp = component;
                        var results = this.dbManager.GetResultsByComponentId(innerComp.EntityId).ToList();

                        ////Here we tell our delegate which method to call.
                        var invokeSearch = new InvokeSolrSearch(searchClient.StartSearch);

                        ////This is our callback method which will be
                        ////called when solr finished the searchrequest.
                        var callback = new AsyncCallback(
                            c =>
                                {
                                    dbMutex.WaitOne();

                                    if (c.IsCompleted)
                                    {
                                        try
                                        {
                                            componentsFinished++;

                                            var resultPot = searchClient.GetResult();

                                            if (resultPot.HasResults)
                                            {
                                                sendResults.AddRange(
                                                    this.GetSendResults(
                                                    resultPot,
                                                    results,
                                                    this.dbManager));

                                                var comp = dbManager.GetEntity(resultPot.ComponentID) as Component;

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

                                    dbMutex.ReleaseMutex();
                                });

                        invokeSearch.BeginInvoke(settings, callback, this);
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
        /// Gets the results we want send per mail as new results.
        /// This method fetches all results from the database of this
        /// component and compares the hash of the content. If the hash
        /// already exists we assume that it is not a new result and
        /// neiher store this result in database nor send an mail.
        /// </summary>
        /// <param name="resultPot">The result pot.</param>
        /// <param name="results">The results.</param>
        /// <param name="dbmanager">The database manager.</param>
        /// <returns>
        /// A list of the results to store in database.
        /// </returns>
        private IEnumerable<Result> GetSendResults(
            SolrResultPot resultPot,
            ICollection<Result> results,
            IDbManager dbmanager)
        {
            var sendResults = new List<Result>();

            foreach (var result in resultPot.Results)
            {
                if (results.Count == 0 || results.All(r => r.ContentHash != result.ContentHash))
                {
                    results.Add(result);
                    sendResults.Add(result);

                    var addEntity = dbmanager.AddEntity(result);
                    result.EntityId = addEntity;

                    Log.InfoFormat(
                    Properties.Resources.SOLR_SEARCH_NEW_RESULT,
                    result);
                }
                else
                {
                    Log.WarnFormat(Properties.Resources.SOLR_SEARCH_KNOWN_RESULT,
                        result);
                }
            }

            return sendResults;
        }

        /// <summary>
        /// Sends the results which we detected as new to the subscribed
        /// mail addresses.
        /// </summary>
        /// <param name="sendResults">The send results.</param>
        /// <param name="orgUnit">The org unit.</param>
        private void SendResults(ICollection<Result> sendResults, OrgUnit orgUnit)
        {
            if (sendResults == null)
            {
                throw new ArgumentNullException("sendResults");
            }

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