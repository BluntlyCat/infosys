// ------------------------------------------------------------------------
// <copyright file="ISolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.SolrClient
{
    using System.ServiceModel;

    /// <summary>
    /// This interface provides methods for controlling Solr.
    /// </summary>
    [ServiceContract]
    public interface ISolrController
    {
        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        [OperationContract]
        void StartSearch(string query);
    }
}
