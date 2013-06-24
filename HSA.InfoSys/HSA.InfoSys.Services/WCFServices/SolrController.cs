// ------------------------------------------------------------------------
// <copyright file="SolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
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
        /// The data base manager.
        /// </summary>
        private IDBManager dbManager = DBManager.ManagerFactory(Guid.NewGuid());

        /// <summary>
        /// Prevents a default instance of the <see cref="SolrController"/> class from being created.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private SolrController(Guid serviceGUID) : base(serviceGUID)
        {
        }

        /// <summary>
        /// Gets the solr controller.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <returns>A new solr controller service.</returns>
        /// <value>
        /// The solr controller.
        /// </value>
        public static SolrController SolrFactory(Guid serviceGUID)
        {
            if (solrController == null)
            {
                solrController = new SolrController(serviceGUID);
            }

            return solrController;
        }

        /// <summary>
        /// Searches for all components of an org unit.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void SearchForOrgUnit(Guid orgUnitGUID)
        {
            this.Search(orgUnitGUID);
        }

        /// <summary>
        /// Searches for one component.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        public void SearchForComponent(Guid componentGUID)
        {
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void Search(Guid orgUnitGUID)
        {
            var controller = new SolrSearchController();
            controller.StartSearch(orgUnitGUID);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }
    }
}
