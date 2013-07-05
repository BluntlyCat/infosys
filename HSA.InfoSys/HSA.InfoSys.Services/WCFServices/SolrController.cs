// ------------------------------------------------------------------------
// <copyright file="SolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Services.LocalServices;

    /// <summary>
    /// In this class are all methods implemented for controlling Solr.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SolrController : Service, ISolrController
    {
        /// <summary>
        /// The solr controller.
        /// </summary>
        private static SolrController solrController;

        /// <summary>
        /// The data base manager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrController"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="dbManager">The db manager.</param>
        private SolrController(Guid serviceGUID, DbManager dbManager) : base(serviceGUID)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// The Solr controller.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        /// <returns>an instance of the Solr controller.</returns>
        public static SolrController SolrFactory(DbManager dbManager)
        {
            return solrController ?? (solrController = new SolrController(Guid.NewGuid(), dbManager));
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
        /// Starts a new search.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        private void Search(Guid orgUnitGUID)
        {
            var controller = new SolrSearchController(this.dbManager);
            controller.StartSearch(orgUnitGUID);
        }

        /// <summary>
        /// Runs this service.
        /// </summary>
        protected override void Run()
        {
        }
    }
}
