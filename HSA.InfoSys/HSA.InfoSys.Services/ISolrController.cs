// ------------------------------------------------------------------------
// <copyright file="ISolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    using System;
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
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        [OperationContract]
        void StartSearch(Guid orgUnitGuid);
    }
}
