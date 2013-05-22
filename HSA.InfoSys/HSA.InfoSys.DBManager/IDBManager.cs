// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager
{
    using System;
    using HSA.InfoSys.Common.DBManager.Data;

    /// <summary>
    /// The interface for accessing the data base.
    /// </summary>
    public interface IDBManager
    {
        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        Guid AddEntity(Entity entity);

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        Guid UpdateEntity(Entity entity);

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <typeparam name="T">The type of what you want.</typeparam>
        /// <param name="entityGuid">The entity GUID.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        T GetEntity<T>(Guid entityGuid) where T : Entity;

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <param name="result">a result object .</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        Component CreateComponent(string name, string category);

        /// <summary>
        /// Creates an source object.
        /// </summary>
        /// <param name="sourceURL">The URL where the source points to.</param>
        /// <returns>
        /// The created source object.
        /// </returns>
        Source CreateSource(string sourceURL);

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">The content of the result</param>
        /// <returns>
        /// The created result object
        /// </returns>
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
        Scheduler CreateScheduler(int days, int hours);
    }
}
