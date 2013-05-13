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
        /// Starts the search.
        /// </summary>
        [OperationContract]
        int StartSearch(string query);

        /// <summary>
        /// Gets the response from solr.
        /// </summary>
        /// <param name="key">The response key.</param>
        /// <returns>The response by key.</returns>
        //[OperationContract]
        //void GetResponse(int key);

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
