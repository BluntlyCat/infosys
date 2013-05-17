// ------------------------------------------------------------------------
// <copyright file="ICrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager.Data;

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

        [OperationContract]
        Guid AddEntity(Entity entity);

        [OperationContract]
        Guid UpdateEntity(Entity entity);

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <typeparam name="T">The type of what you want.</typeparam>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        [OperationContract]
        Entity GetEntity(Guid entityGuid);

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The new component.
        /// </returns>
        [OperationContract]
        Component CreateComponent(string name, string category);

        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="sourceURL">The source URL.</param>
        /// <returns>The new source.</returns>
        [OperationContract]
        Source CreateSource(string sourceURL);
    }
}
