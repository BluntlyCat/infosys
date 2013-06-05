// ------------------------------------------------------------------------
// <copyright file="SolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    using System;
    using System.ServiceModel;
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
        /// Prevents a default instance of the <see cref="SolrController"/> class from being created.
        /// </summary>
        private SolrController()
        {
        }

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="query">The query.</param>
        public delegate void InvokeSolrSearch(string query);

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
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        public void StartSearch(string query)
        {
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SEARCH_STARTED, query);

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
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }
    }
}
