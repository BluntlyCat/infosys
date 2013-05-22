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
        /// <param name="result">The result.</param> 
        /// <returns>
        /// The new component.
        /// </returns>
        [OperationContract]
        Component CreateComponent(string name, string category, Result result);

        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="sourceURL">The source URL.</param>
        /// <returns>The new source.</returns>
        [OperationContract]
        Source CreateSource(string sourceURL);

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">The content of the result</param>
        /// <returns>
        /// The created result object
        /// </returns>
        [OperationContract]
        Result CreateResult(string data);

        /// <summary>
        /// Creates a SystemService object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="component">A component object</param>
        /// <param name="sysconfig">A system config object</param>
        /// <returns>
        /// The created SystemService object
        /// </returns>
        [OperationContract]
        SystemService CreateSystemService(int userId, Component component, SystemConfig sysconfig);

        /// <summary>
        /// Creates a SystemConfig object
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="email">The email text.</param>
        /// <param name="urlActive">if set to <c>true</c> [URL active].</param>
        /// <param name="emailNotification">if set to <c>true</c> [email notification].</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <param name="scheduler">A scheduler object.</param>
        /// <returns>
        /// The created SystemConfig object.
        /// </returns>
        [OperationContract]
        SystemConfig CreateSystemConfig(
            string url,
            string email,
            bool urlActive,
            bool emailNotification,
            bool schedulerActive,
            Scheduler scheduler);

        /// <summary>
        /// Creates a Scheduler object
        /// </summary>
        /// <param name="days">The days.</param>
        /// <param name="hours">The hours.</param>
        /// <returns>
        /// The created Scheduler object.
        /// </returns>
        [OperationContract]
        Scheduler CreateScheduler(int days, int hours);
    }
}
