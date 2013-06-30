// ------------------------------------------------------------------------
// <copyright file="DBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
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
    public class DBManager : Service, IDBManager
    {
        /// <summary>
        /// The logger for db manager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("DBManager");

        /// <summary>
        /// The mutex for data base session.
        /// </summary>
        private static Mutex dbButex = new Mutex();

#if MONO
        /// <summary>
        /// The results if we run this in mono.
        /// </summary>
        private static Result[] results;
#endif

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDBManager dbManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBManager"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private DBManager(Guid serviceGUID) : base(serviceGUID)
        {
        }

        /// <summary>
        /// Gets the opened session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        /// <returns>A NHibernate session object.</returns>
        public static ISession Session
        {
            get
            {
                if (dbManager == null)
                {
                    dbManager = ManagerFactory;
                }

                Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);

                return SessionFactory.OpenSession();
            }
        }

        /// <summary>
        /// Gets the DB manager and ensures that the configuration
        /// will be executed only once and that there is only one db manager.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <returns>
        /// The database manager service.
        /// </returns>
        public static IDBManager ManagerFactory
        {
            get
            {
                dbButex.WaitOne();

                if (dbManager == null)
                {
                    Log.Debug(Properties.Resources.DBMANAGER_NO_MANAGER_FOUND);
                    dbManager = new DBManager(Guid.NewGuid());
                }

                dbButex.ReleaseMutex();

                return dbManager;
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
        /// Gets the settings for.
        /// </summary>
        /// <typeparam name="T">The type of settings.</typeparam>
        /// <returns>
        /// The entity containing the requested settings.
        /// </returns>
        public T GetSettingsFor<T>() where T : Entity
        {
            try
            {
                T setting = null;

                using (ISession session = Session)
                {
                    setting = session.QueryOver<T>()
                    .Where(s => s.Type == typeof(T).Name)
                    .SingleOrDefault() as T;
                }

                return setting;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetSettingsFor<T>()");
            }
        }

        /// <summary>
        /// Gets the mail settings.
        /// </summary>
        /// <returns>
        /// The mail settings.
        /// </returns>
        public EmailNotifierSettings GetMailSettings()
        {
            try
            {
                var setting = this.GetSettingsFor<EmailNotifierSettings>();

                if (setting == null)
                {
                    var newSetting = new EmailNotifierSettings();
                    var guid = dbManager.AddEntity(newSetting);
                    return dbManager.GetEntity(guid) as EmailNotifierSettings;
                }

                return setting;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetMailSettings()");
            }
        }

        /// <summary>
        /// Gets the nutch client settings.
        /// </summary>
        /// <returns>
        /// The nutch client settings.
        /// </returns>
        public NutchControllerClientSettings GetNutchClientSettings()
        {
            try
            {
                var setting = this.GetSettingsFor<NutchControllerClientSettings>();

                if (setting == null)
                {
                    var newSetting = new NutchControllerClientSettings();
                    var guid = dbManager.AddEntity(newSetting);
                    return dbManager.GetEntity(guid) as NutchControllerClientSettings;
                }

                return setting;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetNutchClientSettings()");
            }
        }

        /// <summary>
        /// Gets the solr client settings.
        /// </summary>
        /// <returns>
        /// The solr client settings.
        /// </returns>
        public SolrSearchClientSettings GetSolrClientSettings()
        {
            try
            {
                var setting = this.GetSettingsFor<SolrSearchClientSettings>();

                if (setting == null)
                {
                    var newSetting = new SolrSearchClientSettings();
                    var guid = dbManager.AddEntity(newSetting);
                    return dbManager.GetEntity(guid) as SolrSearchClientSettings;
                }

                return setting;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetSolrClientSettings()");
            }
        }

        /// <summary>
        /// Gets the WCF controller settings.
        /// </summary>
        /// <returns>
        /// The WCF controller settings.
        /// </returns>
        public WCFSettings GetWCFSettings()
        {
            try
            {
                var setting = this.GetSettingsFor<WCFSettings>();

                if (setting == null)
                {
                    var newSetting = new WCFSettings();
                    var guid = dbManager.AddEntity(newSetting);
                    return dbManager.GetEntity(guid) as WCFSettings;
                }

                return setting;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetWCFSettings()");
            }
        }

        #endregion

        /// <summary>
        /// Loads this entities eager.
        /// </summary>
        /// <param name="param">The names of the entities.</param>
        /// <returns>
        /// A list of entities NHibernate must load eager.
        /// </returns>
        public string[] LoadThisEntities(params string[] param)
        {
            List<string> entities = new List<string>();
            var assembly = Assembly.GetAssembly(typeof(Entity));
            var types = assembly.GetTypes();

            Log.InfoFormat(Properties.Resources.DBMANAGER_EAGER_LOAD_THIS_ENTITIES, param);

            foreach (var p in param)
            {
                foreach (var t in types)
                {
                    if (t.Name.Equals(p))
                    {
                        entities.Add(t.Name);
                    }
                }
            }

            return entities.ToArray();
        }

        /// <summary>
        /// Gets all URLs.
        /// </summary>
        /// <returns>
        /// An array containing all URLs for crawling.
        /// </returns>
        public string[] GetAllUrls()
        {
            try
            {
                var urlList = new List<string>();
                var orgUnitConfigs = this.GetOrgUnitConfigurations();

                foreach (var config in orgUnitConfigs)
                {
                    if (config.URLS != null)
                    {
                        var urls = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(config.URLS);

                        foreach (var url in urls)
                        {
                            urlList.Add(url);
                        }
                    }
                }

                return urlList.ToArray();
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetAllUrls()");
            }
        }

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        public Guid AddEntity(Entity entity)
        {
            try
            {
                using (ISession session = Session)
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(entity);
                    transaction.Commit();
                    Log.InfoFormat(Properties.Resources.DBMANAGER_ADD_ENTITY, entity.Type);
                }

                return entity.EntityId;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "AddEntity(Entity entity)");
            }
        }

        /// <summary>
        /// Adds a new Objects (Component, Issue, Source...)
        /// and saves it in database.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void AddEntitys(params Entity[] entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    this.AddEntity(entity);
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "AddEntitys(params Entity[] entities)");
            }
        }

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            try
            {
                using (ISession session = Session)
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(entity);
                        transaction.Commit();
                        Log.InfoFormat(Properties.Resources.DBMANAGER_UPDATE_ENTITY, entity.Type);
                    }

                    return entity.EntityId;
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "UpdateEntity(Entity entity)");
            }
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void DeleteEntity(Entity entity)
        {
            try
            {
                using (ISession session = Session)
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Delete(entity);
                        transaction.Commit();
                        Log.InfoFormat(Properties.Resources.DBMANAGER_DELETE_ENTITY, entity);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "DeleteEntity(Entity entity)");
            }
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public Entity GetEntity(Guid entityGUID, string[] types = null)
        {
            try
            {
                Entity entity;
                using (ISession session = Session)
                using (ITransaction transaction = session.BeginTransaction())
                {
                    entity = session.Get<Entity>(entityGUID);

                    if (types != null)
                    {
                        entity.Unproxy(types);
                    }

                    Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ENTITY, entity.GetType().Name, entity, entityGUID);
                }

                return entity;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetEntity(Guid entityGUID, string[] types = null)");
            }
        }

        /// <summary>
        /// Gets the org units by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="types">The types.</param>
        /// <returns>
        /// A list of org units for the user id.
        /// </returns>
        public OrgUnit[] GetOrgUnitsByUserID(int userID, string[] types = null)
        {
            try
            {
                using (ISession session = Session)
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var orgUnits = session.QueryOver<OrgUnit>()
                        .Where(x => x.UserId == userID)
                        .List<OrgUnit>() as List<OrgUnit>;

                    if (types != null)
                    {
                        foreach (var orgUnit in orgUnits)
                        {
                            orgUnit.Unproxy(types);
                            Log.DebugFormat(Properties.Resources.DBMANAGER_UNPROXY_ORGUNITS, orgUnit.OrgUnitConfig);
                        }
                    }

                    Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ORGUNIT_BY_USERID, orgUnits.OrgUnitsToString(), userID);

                    return orgUnits.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetOrgUnitsByUserID(int userID, string[] types = null)");
            }
        }

        /// <summary>
        /// Gets the components by org unit id.
        /// </summary>
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        /// <returns>
        /// A list of components which belongs to the given OrgUnit.
        /// </returns>
        public Component[] GetComponentsByOrgUnitId(Guid orgUnitGuid)
        {
            try
            {
                using (ISession session = Session)
                {
                    var components = session.QueryOver<Component>()
                        .Where(x => x.OrgUnitGUID == orgUnitGuid)
                        .List<Component>() as List<Component>;

                    Log.InfoFormat(Properties.Resources.DBMANAGER_GET_COMPONENT_BY_ORGUNIT_ID, components.ComponentsToString(), orgUnitGuid);

                    return components.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetComponentsByOrgUnitId(Guid orgUnitGuid)");
            }
        }

        /// <summary>
        /// Gets the results by component id.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns>A list of results which belongs to the given component.</returns>
        public Result[] GetResultsByComponentId(Guid componentGUID)
        {
            try
            {
                using (ISession session = Session)
                {
                    var results = session.QueryOver<Result>()
                        .Where(x => x.ComponentGUID == componentGUID)
                        .List<Result>() as List<Result>;

                    Log.InfoFormat(Properties.Resources.DBMANAGER_GET_RESULTS_BY_COMPONENT_ID, results.ResultsToString(), componentGUID);

                    return results.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetResultsByComponentId(Guid componentGUID)");
            }
        }

        /// <summary>
        /// Gets the scheduler times.
        /// </summary>
        /// <returns>
        /// A list of all scheduler times.
        /// </returns>
        public OrgUnitConfig[] GetOrgUnitConfigurations()
        {
            try
            {
                using (ISession session = Session)
                {
                    var configs = session.QueryOver<OrgUnitConfig>().List<OrgUnitConfig>() as List<OrgUnitConfig>;

                    return configs.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetOrgUnitConfigurations()");
            }
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
        /// Creates a result object
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <param name="content">the content of the result</param>
        /// <param name="url">The source.</param>
        /// <param name="title">The title.</param>
        /// <returns>
        /// the created result object
        /// </returns>
        public Result CreateResult(Guid componentGUID, string content, string url, string title)
        {
            var result = new Result
            {
                ComponentGUID = componentGUID,
                Content = content,
                URL = url,
                Title = title,
                Time = DateTime.Now
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_RESULT, result);

            return result;
        }

        /// <summary>
        /// Creates a OrgUnit object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="name">The system name.</param>
        /// <returns>
        /// The created OrgUnit object
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
        public OrgUnitConfig CreateOrgUnitConfig(
            string urls,
            string emails,
            bool urlActive,
            bool emailNotification,
            int days,
            int time,
            DateTime nextSearch,
            bool schedulerActive)
        {
            if (days <= 0 || time < 0)
            {
                throw new OrgUnitConfigTimeException(this.GetType().Name);
            }

            var orgUnitConfig = new OrgUnitConfig
            {
                URLS = urls,
                Emails = emails,
                URLActive = urlActive,
                EmailActive = emailNotification,
                Days = days,
                Time = time,
                NextSearch = nextSearch,
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
        public List<int> GetResultIndexes(Guid componentGUID)
        {
            try
            {
                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_RESULTS_BY_COMPONENT_ID, componentGUID);

                results = this.GetResultsByComponentId(componentGUID);

                var maxBytes = Math.Pow(2, 15);
                long byteCount = this.GetByteCount(results);
                int requests = this.GetAmountOfRequests(byteCount, maxBytes);
                long byteAmount = 0;

                int index = 0;
                List<int> indexes = new List<int>();
                indexes.Add(0);

                foreach (var result in results)
                {
                    byteAmount += result.SizeOf();

                    if (byteAmount > maxBytes)
                    {
                        var i = index - 1;

                        indexes.Add(i);
                        byteAmount = 0;

                        Log.DebugFormat(Properties.Resources.DBMANAGER_BYTEAMOUNT_GREATER_THAN_MAX_BYTES, byteAmount, maxBytes, i);
                    }

                    index++;
                }

                indexes.Add(results.Length);

                return indexes;
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetResultIndexes(Guid componentGUID)");
            }
        }

        /// <summary>
        /// Gets the index of the results by request.
        /// In this method we fetch the results.
        /// The last index is the first index of the next request so
        /// we begin at the first index and ending one index before the last index.
        /// Otherwise we would fetch the last result two times.
        /// </summary>
        /// <param name="first">The first result index.</param>
        /// <param name="last">The last result index.</param>
        /// <returns>
        /// All results in range of first and the index before last index
        /// </returns>
        public Result[] GetResultsByRequestIndex(int first, int last)
        {
            try
            {
                Log.DebugFormat(Properties.Resources.DBMANAGER_SPLITTED_RESULTS_FROM_TO, first, last);

                var tmp = new Result[last - first];
                var splittedResults = new List<Result>();

                for (int i = first; i < last; i++)
                {
                    try
                    {
                        splittedResults.Add(results[i]);
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

                return splittedResults.ToArray();
            }
            catch (Exception e)
            {
                throw new DBManagerAccessException(e, "GetResultsByRequestIndex(int first, int last)");
            }
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
            if (DBManager.SessionFactory != null && !DBManager.SessionFactory.IsClosed)
            {
                DBManager.SessionFactory.Close();
            }

            base.StopService(cancel);
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
            try
            {
                var configuration = new Configuration();

                configuration.Configure();
                configuration.AddAssembly(typeof(DBManager).Assembly);

                SessionFactory = configuration.BuildSessionFactory();
                new SchemaExport(configuration).Drop(false, false);

                Log.Debug(Properties.Resources.DBSESSION_NHIBERNATE_CONFIG_READY);
            }
            catch (Exception e)
            {
                throw new DBManagerConfigurationException(e, typeof(DBManager).Name);
            }
        }

#if MONO
        /// <summary>
        /// Gets the byte count.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>
        /// The amount of bytes of all results.
        /// </returns>
        private long GetByteCount(Result[] results)
        {
            long byteCount = 0;

            foreach (var result in results)
            {
                byteCount += result.SizeOf();
            }

            Log.DebugFormat(Properties.Resources.DBMANAGER_RESULTS_BYTE_COUNT, byteCount);

            return byteCount;
        }

        /// <summary>
        /// Gets the amount of requests.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <param name="maxBytes">The max bytes.</param>
        /// <returns>The amount of necessary requests.</returns>
        private int GetAmountOfRequests(double byteCount, double maxBytes)
        {
            var tmp = byteCount / maxBytes;

            if (tmp % 2 != 0)
            {
                tmp++;
                Log.Debug(Properties.Resources.DBMANAGER_INCREASE_REQUEST_AMOUNT);
            }

            Log.DebugFormat(Properties.Resources.DBMANAGER_TOTAL_REQUEST_AMOUNT, tmp);

            return (int)tmp;
        }
#endif
    }
}