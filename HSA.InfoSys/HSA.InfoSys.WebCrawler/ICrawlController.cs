// ------------------------------------------------------------------------
// <copyright file="ICrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System.ServiceModel;

    /// <summary>
    /// This is the interface for communication between the GUI and the web crawler
    /// If we change something in this interface we have to create a new Proxy.cs
    /// class by typing the following command:
    /// svcutil http://localhost:8085/GetMetaInformation /out:Proxy.cs
    /// But there is also a script called updateProxy in the main project folder for doing this.
    /// </summary>
    [ServiceContract]
    public interface ICrawlController
    {
        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        [OperationContract]
        void StartSearch(string query);

        /// <summary>
        /// Starts the services.
        /// </summary>
        [OperationContract]
        void StartServices();

        /// <summary>
        /// Stops the services.
        /// </summary>
        [OperationContract]
        void StopServices();
    }
}
