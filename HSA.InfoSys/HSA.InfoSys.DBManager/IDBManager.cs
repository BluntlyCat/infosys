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
        Component CreateComponent(string name, string category, Result result);

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
        /// <param name="data">the content of the result</param>
        /// <returns>
        /// the created result object
        /// </returns>
        Result CreateResult(string data);

        /// <summary>
        /// Creates a SystemService object
        /// </summary>
        /// <param name="component">a component object </param>
        /// <param name="sysconfig">a systemconfig object</param>
        /// <returns>
        /// the created SystemService object
        /// </returns>
        SystemService CreateSystemService(int UserId, Component component, SystemConfig sysconfig);

        /// <summary>
        /// Creates a SystemConfig object
        /// </summary>
        /// <param name="URL">the URL</param>
        /// <param name="Email">the Emailtext</param>
        /// <param name="URLActive">accessibility of the URL</param>
        /// <param name="EmailNotification">the EmailNotification </param>
        /// <param name="SchedulerActive">accessibility of the scheduler</param>
        /// <param name="scheduler">a scheduler object</param>
        /// <returns>the created SystemConfig object</returns>
        SystemConfig CreateSystemConfig(string URL, string Email, bool URLActive,
            bool EmailNotification, bool SchedulerActive, Scheduler scheduler);

        /// <summary>
        /// Creates a Scheduler object
        /// </summary>
        /// <param name="Days">the days for scheduling</param>
        /// <param name="Hours">the hours for Scheuduling</param>
        /// <returns>the created Scheduler object</returns>
        Scheduler CreateScheduler(int Days, int Hours);
    }
}
