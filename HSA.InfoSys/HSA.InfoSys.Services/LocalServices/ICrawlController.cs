// ------------------------------------------------------------------------
// <copyright file="ICrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// This is the interface for communication between the GUI and the web crawler
    /// </summary>
    [ServiceContract]
    public interface ICrawlController
    {
        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <param name="type">The type of the service to start.</param>
        /// <returns>
        /// True indicates that the service is started.
        /// </returns>
        [OperationContract]
        bool StartService(Type type);

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="type">The type of the service to stop.</param>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <returns>
        /// False indicates that the service is stopped.
        /// </returns>
        [OperationContract]
        bool StopService(Type type, bool cancel = false);

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
