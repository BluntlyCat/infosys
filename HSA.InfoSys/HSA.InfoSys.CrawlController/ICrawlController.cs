// ------------------------------------------------------------------------
// <copyright file="ICrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System.ServiceModel;
    using HSA.InfoSys.Common.Services;

    /// <summary>
    /// This is the interface for communication between the GUI and the web crawler
    /// </summary>
    [ServiceContract]
    public interface ICrawlController
    {
        /// <summary>
        /// Registers the service.
        /// </summary>
        /// <param name="service">The service.</param>
        void RegisterService(IService service);

        /// <summary>
        /// Starts the services.
        /// </summary>
        /// <returns>True indicates that the services are started.</returns>
        [OperationContract]
        bool StartServices();

        /// <summary>
        /// Stops the services.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <returns>False indicates that the services are stopped.</returns>
        [OperationContract]
        bool StopServices(bool cancel = false);
    }
}
