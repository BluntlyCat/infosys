// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
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
        /// Adds a new Object (Component, Issue, Source...)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Guid AddEntity(Entity entity);

        /// <summary>
        /// Adds a new Objects (Component, Issue, Source...)
        /// and saves it in database.
        /// </summary>
        /// <param name="entities">The entities.</param>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        void AddEntitys(params Entity[] entities);

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
        /// Gets the components by org unit id.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <returns>
        /// A list of components which belongs to the given OrgUnit.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Component[] GetComponentsByOrgUnitId(Guid orgUnitGUID);

        /// <summary>
        /// Gets the results by component id.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns>A list of results which belongs to the given component.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Result[] GetResultsByComponentId(Guid componentGUID);

        /// <summary>
        /// Gets the scheduler times.
        /// </summary>
        /// <returns>A list of all OrgUnitConfig objects.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnitConfig[] GetOrgUnitConfigurations();

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Component CreateComponent(string name, Guid orgUnitGUID);

        /// <summary>
        /// Creates the result.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <param name="content">The content.</param>
        /// <param name="url">The URL.</param>
        /// <param name="title">The title.</param>
        /// <returns>
        /// A result object of for a web crawl.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Result CreateResult(Guid componentGUID, string content, string url, string title);

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
        /// <param name="days">The days.</param>
        /// <param name="time">The time.</param>
        /// <param name="nextSearch">The next search.</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
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
            int days,
            int time,
            DateTime nextSearch,
            bool schedulerActive);
    }
}
