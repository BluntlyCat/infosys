// ------------------------------------------------------------------------
// <copyright file="ISearchRecall.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.NetDataContractSerializer;

    /// <summary>
    /// This interface provides only one method
    /// for calling the GUI to indicate that a
    /// search request for an org unit is finished.
    /// </summary>
    [ServiceContract]
    public interface IGUIRecall
    {
        /// <summary>
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="results">The results.</param>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        void SearchRecall(Guid orgUnitGUID, Result[] results);

        /// <summary>
        /// Called if the crawl failed.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        void CrawlFailedRecall(Guid orgUnitGUID);
    }
}
