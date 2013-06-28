// ------------------------------------------------------------------------
// <copyright file="ICrawlerService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System.ServiceModel;
    using HSA.InfoSys.Common.NetDataContractSerializer;

    /// <summary>
    /// This interface provides control mechanism for this service.
    /// </summary>
    [ServiceContract]
    public interface ICrawlerService
    {
        /// <summary>
        /// Stops the services.
        /// </summary>
        [OperationContract]
        [UseNetDataContractSerializer]
        void StopServices();

        /// <summary>
        /// Shutdown the crawler.
        /// </summary>
        [OperationContract]
        [UseNetDataContractSerializer]
        void ShutdownCrawler();
    }
}
