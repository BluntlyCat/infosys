// ------------------------------------------------------------------------
// <copyright file="DBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Extensions;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// The DBManager handles database requests.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DbManager : Service, IDbManager
    {
        /// <summary>
        /// The logger for db manager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("DbManager");

        /// <summary>
        /// The mutex for data base session.
        /// </summary>
        private static readonly Mutex DbMutex = new Mutex();

        /// <summary>
        /// The get result indexes mutex.
        /// </summary>
        private static readonly Mutex GetResultIndexesMutex = new Mutex();

        /// <summary>
        /// The get result by index mutex.
        /// </summary>
        private static readonly Mutex GetResultByIndexMutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDbManager manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbManager"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private DbManager(Guid serviceGUID) : base(serviceGUID)
        {
        }

        /// <summary>
        /// Gets the opened session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        /// <returns>A NHibernate session object.</returns>
        private static ISession Session
        {
            get
            {
                if (manager == null)
                {
                    manager = ManagerFactory;
                }

                Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);

                return SessionFactory.OpenSession();
            }
        }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>
        /// The database manager.
        /// </value>
        public static IDbManager ManagerFactory
        {
            get
            {
                DbMutex.WaitOne();

                if (manager == null)
                {
                    Log.Debug(Properties.Resources.DBMANAGER_NO_MANAGER_FOUND);
                    manager = new DbManager(Guid.NewGuid());
                }

                DbMutex.ReleaseMutex();

                return manager;
            }
        }

        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        private static ISessionFactory SessionFactory { get; set; }

        #region Settings

        /// <summary>
        /// Gets the settings for the given type.
        /// </summary>
        /// <typeparam name="T">The type of settings.</typeparam>
        /// <returns>
        /// The entity containing the requested settings.
        /// </returns>
        private static T GetSettingsFor<T>() where T : Entity
        {
            try
            {
                using (var session = Session)
                {
                    return session.QueryOver<T>()
                        .Where(s => s.Type == typeof(T).Name)
                        .SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetSettingsFor<T>()");
            }
        }

        /// <summary>
        /// Gets the mail settings.
        /// If there are none this method creates one
        /// and initializes them with default values.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// The mail settings.
        /// </returns>
        public EmailNotifierSettings GetMailSettings()
        {
            DbMutex.WaitOne();

            EmailNotifierSettings settings;

            try
            {
                settings = GetSettingsFor<EmailNotifierSettings>();

                if (settings == null)
                {
                    var newSetting = new EmailNotifierSettings();
                    newSetting.SetDefaults();
                    var guid = manager.AddEntity(newSetting);
                    settings = manager.GetEntity(guid) as EmailNotifierSettings;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetMailSettings()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return settings;
        }

        /// <summary>
        /// Gets the nutch client settings.
        /// If there are none this method creates one
        /// and initializes them with default values.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// The nutch client settings.
        /// </returns>
        public NutchControllerClientSettings GetNutchClientSettings()
        {
            DbMutex.WaitOne();

            NutchControllerClientSettings settings;

            try
            {
                settings = GetSettingsFor<NutchControllerClientSettings>();

                if (settings == null)
                {
                    var newSetting = new NutchControllerClientSettings();
                    newSetting.SetDefaults();
                    var guid = manager.AddEntity(newSetting);
                    settings = manager.GetEntity(guid) as NutchControllerClientSettings;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetNutchClientSettings()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return settings;
        }

        /// <summary>
        /// Gets the solr client settings.
        /// If there are none this method creates one
        /// and initializes them with default values.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// The solr client settings.
        /// </returns>
        public SolrSearchClientSettings GetSolrClientSettings()
        {
            DbMutex.WaitOne();

            SolrSearchClientSettings settings;

            try
            {
                settings = GetSettingsFor<SolrSearchClientSettings>();

                if (settings == null)
                {
                    var newSetting = new SolrSearchClientSettings();
                    newSetting.SetDefaults();
                    var guid = manager.AddEntity(newSetting);
                    settings = manager.GetEntity(guid) as SolrSearchClientSettings;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetSolrClientSettings()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return settings;
        }

        /// <summary>
        /// Gets the WCF controller settings.
        /// If there are none this method creates one
        /// and initializes them with default values.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// The WCF controller settings.
        /// </returns>
        public WCFSettings GetWcfSettings()
        {
            DbMutex.WaitOne();

            WCFSettings settings;

            try
            {
                settings = GetSettingsFor<WCFSettings>();

                if (settings == null)
                {
                    var newSetting = new WCFSettings();
                    newSetting.SetDefaults();
                    var guid = manager.AddEntity(newSetting);
                    settings = manager.GetEntity(guid) as WCFSettings;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetWCFSettings()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return settings;
        }

        #endregion

        /// <summary>
        /// Loads this instances from NHibernate eager.
        /// NHibernate supports lazy loading, so we need some
        /// functionality to load a reference to a foreign table too.
        /// </summary>
        /// <param name="param">The names of the entities.</param>
        /// <returns>
        /// A list of entities NHibernate must load eager.
        /// </returns>
        public string[] LoadThisEntities(params object[] param)
        {
            var assembly = Assembly.GetAssembly(typeof(Entity));
            var types = assembly.GetTypes();

            Log.InfoFormat(Properties.Resources.DBMANAGER_EAGER_LOAD_THIS_ENTITIES, param);

            return (from p in param from t in types where t.Name.Equals(p) select t.Name).ToArray();
        }

        /// <summary>
        /// Gets all URLs.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// An array containing all URLs for crawling.
        /// </returns>
        public string[] GetAllUrls()
        {
            DbMutex.WaitOne();

            var urlList = new List<string>();

            try
            {
                var orgUnitConfigs = this.GetOrgUnitConfigurations();

                if (orgUnitConfigs != null)
                {
                    foreach (var config in orgUnitConfigs)
                    {
                        if (config.URLs == null)
                        {
                            continue;
                        }

                        var urls = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(config.URLs);

                        urlList.AddRange(urls);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetAllUrls()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return urlList.ToArray();
        }

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>The GUID of the added entity.</returns>
        public Guid AddEntity(Entity entity)
        {
            DbMutex.WaitOne();

            try
            {
                if (Session != null)
                {
                    using (var session = Session)
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            session.Save(entity);
                            transaction.Commit();
                            Log.InfoFormat(Properties.Resources.DBMANAGER_ADD_ENTITY, entity.Type);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "AddEntity(Entity entity)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return entity.EntityId;
        }

        /// <summary>
        /// Adds new Objects (Component, Issue, Source...)
        /// and saves it in database.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        public void AddEntitys(params Entity[] entities)
        {
            DbMutex.WaitOne();

            try
            {
                if (entities != null)
                {
                    foreach (var entity in entities)
                    {
                        this.AddEntity(entity);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "AddEntitys(params Entity[] entities)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            DbMutex.WaitOne();

            try
            {
                if (Session != null)
                {
                    using (var session = Session)
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            session.Update(entity);
                            transaction.Commit();
                            Log.InfoFormat(Properties.Resources.DBMANAGER_UPDATE_ENTITY, entity.Type);
                        }
                    }
                }
                else
                {
                    return new Guid();
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "UpdateEntity(Entity entity)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return entity.EntityId;
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        public void DeleteEntity(Entity entity)
        {
            DbMutex.WaitOne();

            try
            {
                if (Session == null)
                {
                    return;
                }

                using (var session = Session)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.Delete(entity);
                        transaction.Commit();
                        Log.InfoFormat(Properties.Resources.DBMANAGER_DELETE_ENTITY, entity);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "DeleteEntity(Entity entity)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <param name="types">The types you want load eager.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public Entity GetEntity(Guid entityGUID, string[] types = null)
        {
            DbMutex.WaitOne();

            Entity entity;

            try
            {
                using (var session = Session)
                {
                    entity = session.Get<Entity>(entityGUID);

                    if (entity != null)
                    {
                        if (types != null)
                        {
                            entity.Unproxy(types);
                        }

                        Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ENTITY, entity.GetType().Name, entity, entityGUID);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetEntity(Guid entityGUID, string[] types = null)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return entity;
        }

        /// <summary>
        /// Gets the org units by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="types">The types.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// A list of org units for the user id.
        /// </returns>
        public OrgUnit[] GetOrgUnitsByUserId(int userId, string[] types = null)
        {
            DbMutex.WaitOne();

            List<OrgUnit> orgUnits;

            try
            {
                using (var session = Session)
                {
                    orgUnits = session.QueryOver<OrgUnit>()
                        .Where(x => x.UserId == userId)
                        .List<OrgUnit>() as List<OrgUnit>;

                    if (types != null)
                    {
                        if (orgUnits != null)
                        {
                            foreach (var orgUnit in orgUnits)
                            {
                                orgUnit.Unproxy(types);
                                Log.DebugFormat(Properties.Resources.DBMANAGER_UNPROXY_ORGUNITS, orgUnit.OrgUnitConfig);
                            }
                        }
                    }

                    Log.InfoFormat(
                        Properties.Resources.DBMANAGER_GET_ORGUNIT_BY_USERID, orgUnits.OrgUnitsToString(),
                        userId);
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetOrgUnitsByUserID(int userID, string[] types = null)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return orgUnits != null ? orgUnits.ToArray() : new OrgUnit[0];
        }

        /// <summary>
        /// Gets the org unit GUID by org unit config GUID.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The org unit config GUID.</param>
        /// <param name="types">The types.</param>
        /// <returns>
        /// The OrgUnitGUID of the OrgUnit belonging to the OrgUnitConfigGUID.
        /// </returns>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        public Guid GetOrgUnitGuidByOrgUnitConfigGuid(Guid orgUnitConfigGUID, string[] types = null)
        {
            DbMutex.WaitOne();

            Guid orgUnitGuid;

            try
            {
                using (var session = Session)
                {
                    orgUnitGuid = session.QueryOver<OrgUnit>()
                        .Where(u => u.OrgUnitConfig.EntityId == orgUnitConfigGUID)
                        .SingleOrDefault().EntityId;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetOrgUnitsByUserID(int userID, string[] types = null)");
            }
            finally
            {
                DbMutex.ReleaseMutex();   
            }

            return orgUnitGuid;
        }

        /// <summary>
        /// Gets the org unit configurations by active scheduler.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// A list of Orgunit configurations where the Scheduler is active.
        /// </returns>
        public OrgUnitConfig[] GetOrgUnitConfigsByActiveScheduler(string[] types = null)
        {
            DbMutex.WaitOne();

            IList<OrgUnitConfig> configs;

            try
            {
                using (var session = Session)
                {
                    configs = session.QueryOver<OrgUnitConfig>()
                                     .Where(x => x.SchedulerActive)
                                     .List<OrgUnitConfig>();

                    if (configs == null)
                    {
                        throw new ArgumentNullException("types");
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetOrgUnitsByUserID(int userID, string[] types = null)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return configs.ToArray();
        }

        /// <summary>
        /// Gets the components by org unit id.
        /// </summary>
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// A list of components which belongs to the given OrgUnit.
        /// </returns>
        public Component[] GetComponentsByOrgUnitId(Guid orgUnitGuid)
        {
            DbMutex.WaitOne();

            List<Component> components;

            try
            {
                using (var session = Session)
                {
                    components = session.QueryOver<Component>()
                        .Where(x => x.OrgUnitGUID == orgUnitGuid)
                        .List<Component>() as List<Component>;

                    if (components != null)
                    {
                        Log.InfoFormat(
                            Properties.Resources.DBMANAGER_GET_COMPONENT_BY_ORGUNIT_ID,
                            components.ComponentsToString(),
                            orgUnitGuid);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetComponentsByOrgUnitId(Guid orgUnitGuid)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return components != null ? components.ToArray() : new Component[0];
        }

        /// <summary>
        /// Gets the results by component id.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>A list of results which belongs to the given component.</returns>
        public Result[] GetResultsByComponentId(Guid componentGUID)
        {
            DbMutex.WaitOne();

            List<Result> results;

            try
            {
                using (var session = Session)
                {
                    results = session.QueryOver<Result>()
                        .Where(x => x.ComponentGUID == componentGUID)
                        .List<Result>() as List<Result>;

                    if (results != null)
                    {
                        Log.InfoFormat(
                            Properties.Resources.DBMANAGER_GET_RESULTS_BY_COMPONENT_ID,
                            results.Count,
                            componentGUID);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetResultsByComponentId(Guid componentGUID)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return results != null ? results.ToArray() : new Result[0];
        }

        /// <summary>
        /// Gets the scheduler times.
        /// </summary>
        /// <exception cref="DbManagerAccessException">Thrown on error while accessing the database.</exception>
        /// <returns>
        /// <returns>A list of all OrgUnitConfig objects.</returns>
        /// </returns>
        public OrgUnitConfig[] GetOrgUnitConfigurations()
        {
            DbMutex.WaitOne();

            List<OrgUnitConfig> configs;

            try
            {
                using (var session = Session)
                {
                    configs = session.QueryOver<OrgUnitConfig>()
                        .List<OrgUnitConfig>() as List<OrgUnitConfig>;
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetOrgUnitConfigurations()");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return configs != null ? configs.ToArray() : new OrgUnitConfig[0];
        }

        /// <summary>
        /// Gets the emails by urls.
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns>
        /// A list of email addresses according to the url of an OrgUnitConfig.
        /// </returns>
        public string[] GetEmailsByUrls(string[] urls)
        {
            DbMutex.WaitOne();

            IEnumerable<string> addresses;

            try
            {
                using (var session = Session)
                {
                    addresses = session.QueryOver<OrgUnitConfig>()
                        .Where(c => c.URLs.Equals(urls.ElementsToString()))
                        .List<string>().Distinct();
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetEmailsByUrls(string[] urls)");
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }

            return addresses.ToArray();
        }

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        public Component CreateComponent(string componentName, Guid orgUnitGUID)
        {
            var component = new Component
            {
                Name = componentName,
                OrgUnitGUID = orgUnitGUID
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_COMPONENT, component);

            return component;
        }

        /// <summary>
        /// Creates a OrgUnit object.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="name">The system name.</param>
        /// <returns>
        /// The created OrgUnit object.
        /// </returns>
        public OrgUnit CreateOrgUnit(int userId, string name)
        {
            var orgUnit = new OrgUnit
            {
                UserId = userId,
                Name = name
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_ORGUNIT, orgUnit);

            return orgUnit;
        }

        /// <summary>
        /// Creates a OrgUnitConfig object.
        /// </summary>
        /// <param name="urls">The URL.</param>
        /// <param name="emails">The email text.</param>
        /// <param name="emailNotification">if set to <c>true</c> [email notification].</param>
        /// <param name="days">The days.</param>
        /// <param name="time">The time.</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <returns>
        /// The created OrgUnitConfig object.
        /// </returns>
        public OrgUnitConfig CreateOrgUnitConfig(
            string urls,
            string emails,
            bool emailNotification,
            int days,
            int time,
            bool schedulerActive)
        {
            if (days <= 0 || time < 0)
            {
                throw new OrgUnitConfigTimeException(this.GetType().Name);
            }

            var orgUnitConfig = new OrgUnitConfig
            {
                URLs = urls,
                Emails = emails,
                EmailActive = emailNotification,
                Days = days,
                Time = time,
                SchedulerActive = schedulerActive
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_ORGUNITCONFIG, orgUnitConfig);

            return orgUnitConfig;
        }

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
        /// <exception cref="DbManagerAccessException">GetResultIndexes(Guid componentGUID)</exception>
        public int[] GetResultIndexes(Guid componentGUID)
        {
            GetResultIndexesMutex.WaitOne();

            try
            {
                var results = this.GetResultsByComponentId(componentGUID);
                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_RESULTS_BY_COMPONENT_ID, results.Length, componentGUID);

                var maxBytes = Math.Pow(2, 15);
                var byteAmount = 0L;

                var index = 0;
                var indexes = new List<int> {0};

                foreach (var result in results)
                {
                    byteAmount += result.SizeOf();

                    if (byteAmount > maxBytes)
                    {
                        var i = index - 1;

                        indexes.Add(i);
                        byteAmount = 0;

                        Log.DebugFormat(
                            Properties.Resources.DBMANAGER_BYTEAMOUNT_GREATER_THAN_MAX_BYTES,
                            byteAmount,
                            maxBytes,
                            i);
                    }

                    index++;
                }

                indexes.Add(results.Length);

                return indexes.ToArray();
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetResultIndexes(Guid componentGUID)");
            }
            finally
            {
                GetResultIndexesMutex.ReleaseMutex();
            }
        }

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
        /// <exception cref="DbManagerAccessException">GetResultsByRequestIndex(int first, int last)</exception>
        public Result[] GetResultsByRequestIndex(Guid componentGUID, int first, int last)
        {
            GetResultByIndexMutex.WaitOne();

            var splittedResults = new List<Result>();

            try
            {
                Log.DebugFormat(Properties.Resources.DBMANAGER_SPLITTED_RESULTS_FROM_TO, first, last);

                var allResults = this.GetResultsByComponentId(componentGUID);

                for (var i = first; i < last; i++)
                {
                    try
                    {
                        splittedResults.Add(allResults[i]);
                    }
                    catch (IndexOutOfRangeException ior)
                    {
                        Log.ErrorFormat(Properties.Resources.LOG_INDEX_OUT_OF_RANGE, ior);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbManagerAccessException(e, "GetResultsByRequestIndex(int first, int last)");
            }
            finally
            {
                GetResultByIndexMutex.ReleaseMutex();
            }

            return splittedResults.ToArray();
        }
#endif

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            ConfigureSession();
            base.StartService();

            Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            DbMutex.WaitOne();

            if (SessionFactory != null && !SessionFactory.IsClosed)
            {
                SessionFactory.Close();
            }

            base.StopService(cancel);

            DbMutex.ReleaseMutex();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }

        /// <summary>
        /// Configures the session.
        /// </summary>
        private static void ConfigureSession()
        {
            DbMutex.WaitOne();

            try
            {
                var configuration = new Configuration();

                configuration.Configure();
                configuration.AddAssembly(typeof (DbManager).Assembly);

                SessionFactory = configuration.BuildSessionFactory();
                new SchemaExport(configuration).Drop(false, false);

                Log.Debug(Properties.Resources.DBSESSION_NHIBERNATE_CONFIG_READY);
            }
            catch (Exception e)
            {
                throw new DbManagerConfigurationException(e, typeof (DbManager).Name);
            }
            finally
            {
                DbMutex.ReleaseMutex();
            }
        }
    }
}