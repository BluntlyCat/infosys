// ------------------------------------------------------------------------
// <copyright file="ICrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.DBManager.Data;

    /// <summary>
    /// This is the interface for communication between the GUI and the web crawler
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

        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The GUID of the new entity.</returns>
        [OperationContract]
        Guid AddEntity(Entity entity);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The GUID of the updated entity.</returns>
        [OperationContract]
        Guid UpdateEntity(Entity entity);

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGuid">The entity GUID.</param>
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
