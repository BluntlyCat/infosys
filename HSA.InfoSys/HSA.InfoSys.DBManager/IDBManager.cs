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
    using HSA.InfoSys.Common.NetDataContractSerializer;

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
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        string[] LoadThisEntities(params string[] param);

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Guid AddEntity(Entity entity);

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Guid UpdateEntity(Entity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        void DeleteEntity(Entity entity);

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGuid">The entity GUID.</param>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Entity GetEntity(Guid entityGuid, string[] types = null);

        /// <summary>
        /// Gets the org units by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>A list of org units for the user id.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnit[] GetOrgUnitsByUserID(int userID);

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Component CreateComponent(string name, OrgUnit orgUnit);

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">The content of the result</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The created result object
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Result CreateResult(string data, string source);

        /// <summary>
        /// Creates a OrgUnit object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="name">The system name.</param>
        /// <returns>
        /// The created OrgUnit object
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnit CreateOrgUnit(int userId, string name);

        /// <summary>
        /// Creates a OrgUnitConfig object
        /// </summary>
        /// <param name="urls">The URL.</param>
        /// <param name="emails">The email text.</param>
        /// <param name="urlActive">if set to <c>true</c> [URL active].</param>
        /// <param name="emailNotification">if set to <c>true</c> [email notification].</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <param name="scheduler">A scheduler object.</param>
        /// <returns>
        /// The created OrgUnitConfig object.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnitConfig CreateOrgUnitConfig(
            string urls,
            string emails,
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
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Scheduler CreateScheduler(int days, int hours);
    }
}
