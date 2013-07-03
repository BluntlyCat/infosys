// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
#define MONO
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
#if MONO
    using System.Collections.Generic;
#endif
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.NetDataContractSerializer;

    /// <summary>
    /// The interface for accessing the data base.
    /// </summary>
    [ServiceContract]
    public interface IDbManager
    {
        /// <summary>
        /// Gets the mail settings.
        /// </summary>
        /// <returns>The mail settings.</returns>
        [OperationContract]
        [UseNetDataContractSerializer]
        EmailNotifierSettings GetMailSettings();

        /// <summary>
        /// Gets the nutch client settings.
        /// </summary>
        /// <returns>The nutch client settings.</returns>
        [OperationContract]
        [UseNetDataContractSerializer]
        NutchControllerClientSettings GetNutchClientSettings();

        /// <summary>
        /// Gets the solr client settings.
        /// </summary>
        /// <returns>
        /// The solr client settings.
        /// </returns>
        [OperationContract]
        [UseNetDataContractSerializer]
        SolrSearchClientSettings GetSolrClientSettings();

        /// <summary>
        /// Gets the WCF controller settings.
        /// </summary>
        /// <returns>The WCF controller settings.</returns>
        [OperationContract]
        [UseNetDataContractSerializer]
        WCFSettings GetWcfSettings();

        /// <summary>
        /// Loads this entities eager.
        /// </summary>
        /// <param name="param">The names of the entities.</param>
        /// <returns>
        /// A list of entities NHibernate must load eager.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        string[] LoadThisEntities(params object[] param);

        /// <summary>
        /// Gets all URLs.
        /// </summary>
        /// <returns>An array containing all URLs for crawling.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        string[] GetAllUrls();

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
        /// <param name="userId">The user ID.</param>
        /// <param name="types">The types.</param>
        /// <returns>
        /// A list of org units for the user id.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnit[] GetOrgUnitsByUserId(int userId, string[] types = null);

        /// <summary>
        /// Gets the org unit GUID by org unit config GUID.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The org unit config GUID.</param>
        /// <param name="types">The types.</param>
        /// <returns>The OrgUnitGUID of the OrgUnit belonging to the OrgUnitConfigGUID.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Guid GetOrgUnitGuidByOrgUnitConfigGuid(Guid orgUnitConfigGUID, string[] types = null);

        /// <summary>
        /// Gets the org unit configurations by active scheduler.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>A list of Orgunit configurations where the Scheduler is active.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        OrgUnitConfig[] GetOrgUnitConfigsByActiveScheduler(string[] types = null);

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
        /// Gets the emails by urls.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>A list of email addresses according to the url of an OrgUnitConfig.</returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        string[] GetEmailsByUrls(string[] urls);

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

#if MONO
        /// <summary>
        /// Gets the list of indexes of results.
        /// In MONO we only can send 2^16 Bytes because of a
        /// MONO intern restriction, so we need to split the
        /// results into more than one request to fetch all
        /// results of this component.
        /// Each couple of indexes includes a range of results
        /// whose size is in range of 2^15 bytes because we will
        /// need some space for serialisation too. A couple of
        /// indexes is the first and the next index in this list.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns>
        /// A list of indexes.
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        int[] GetResultIndexes(Guid componentGUID);

        /// <summary>
        /// Gets the index of the results by request.
        /// In this method we fetch the results.
        /// The last index is the first index of the next request so
        /// we begin at the first index and ending one index before the last index.
        /// Otherwise we would fetch the last result two times.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <param name="first">The first result index.</param>
        /// <param name="last">The last result index.</param>
        /// <returns>
        /// All results in range of first and the index before last index
        /// </returns>
        [UseNetDataContractSerializer]
        [OperationContractAttribute]
        Result[] GetResultsByRequestIndex(Guid componentGUID, int first, int last);
#endif
    }
}
