// ------------------------------------------------------------------------
// <copyright file="ISolrController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
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
        /// Searches for all components of an org unit.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        [OperationContract]
        void SearchForOrgUnit(Guid orgUnitGUID);
    }
}
