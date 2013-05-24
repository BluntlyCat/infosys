// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using HSA.InfoSys.Common.DBManager.Data;

    /// <summary>
    /// The interface for accessing the data base.
    /// </summary>
    [ServiceContract]
    public interface IDBManager
    {
        /// <summary>
        /// Loads this entities eager.
        /// </summary>
        /// <param name="param">The names of the entities.</param>
        /// <returns>
        /// A list of entities NHibernate must load eager.
        /// </returns>
        [OperationContract]
        List<Type> LoadThisEntities(params string[] param);

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        [OperationContract]
        Guid AddEntity(Entity entity);

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        [OperationContract]
        Guid UpdateEntity(Entity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        [OperationContract]
        void DeleteEntity(Entity entity);

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGuid">The entity GUID.</param>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        [OperationContract]
        Entity GetEntity(Guid entityGuid, List<Type> types = null);


        /// <summary>
        /// Gets the org units by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        [OperationContract]
        IList<OrgUnit> GetOrgUnitsByUserID(int userID);

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        [OperationContract]
        Component CreateComponent(string name, OrgUnit orgUnit);

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">The content of the result</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The created result object
        /// </returns>
        [OperationContract]
        Result CreateResult(string data, string source);

        /// <summary>
        /// Creates a OrgUnit object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="name">The system name.</param>
        /// <returns>
        /// The created OrgUnit object
        /// </returns>
        [OperationContract]
        OrgUnit CreateOrgUnit(int userId, string name);

        /// <summary>
        /// Creates a OrgUnitConfig object
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="email">The email text.</param>
        /// <param name="urlActive">if set to <c>true</c> [URL active].</param>
        /// <param name="emailNotification">if set to <c>true</c> [email notification].</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <param name="scheduler">A scheduler object.</param>
        /// <returns>
        /// The created OrgUnitConfig object.
        /// </returns>
        [OperationContract]
        OrgUnitConfig CreateOrgUnitConfig(
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
