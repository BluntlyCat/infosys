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
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Exceptions;
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
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDBManager dbManager;

        /// <summary>
        /// Prevents a default instance of the <see cref="DBManager"/> class from being created.
        /// </summary>
        private DBManager()
        {
        }

        /// <summary>
        /// Gets the DB manager and ensures that the configuration
        /// will be executed only once and that there is only one db manager.
        /// </summary>
        /// <returns>
        /// The Database Manager
        /// </returns>
        public static IDBManager ManagerFactory
        {
            get
            {
                if (dbManager == null)
                {
                    Log.Debug(Properties.Resources.DBMANAGER_NO_MANAGER_FOUND);
                    dbManager = new DBManager();
                }

                return dbManager;
            }
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
                if (SessionFactory == null)
                {
                    ConfigureSession();
                }

                Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);

                return SessionFactory.OpenSession();
            }
        }

        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        private static ISessionFactory SessionFactory { get; set; }

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
            var assembly = Assembly.GetAssembly(typeof(DBManager));
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
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        public Guid AddEntity(Entity entity)
        {
            using (ISession session = Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(entity);
                transaction.Commit();
                Log.Info(Properties.Resources.DBMANAGER_ADD_ENTITY);
            }

            return entity.EntityId;
        }

        /// <summary>
        /// Adds a new Objects (Component, Issue, Source...)
        /// and saves it in database.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void AddEntitys(params Entity[] entities)
        {
            foreach (var entity in entities)
            {
                using (ISession session = Session)
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(entity);
                    transaction.Commit();
                    Log.Info(Properties.Resources.DBMANAGER_ADD_ENTITY);
                }
            }
        }

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            using (ISession session = Session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entity);
                    transaction.Commit();
                    Log.Info(Properties.Resources.DBMANAGER_UPDATE_ENTITY);
                }

                return entity.EntityId;
            }
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void DeleteEntity(Entity entity)
        {
            using (ISession session = Session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(entity);
                    transaction.Commit();
                    Log.Info(Properties.Resources.DBMANAGER_DELETE_ENTITY);
                }
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
            Entity entity;
            using (ISession session = Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                entity = session.Get<Entity>(entityGUID);

                if (types != null)
                {
                    entity.Unproxy(types);
                }

                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ENTITY, entity.GetType(), entity, entityGUID);
            }

            return entity;
        }

        /// <summary>
        /// Gets the org units by user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>
        /// A list of org units for the user id.
        /// </returns>
        public OrgUnit[] GetOrgUnitsByUserID(int userID)
        {
            using (ISession session = Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                var orgUnit = session.QueryOver<OrgUnit>()
                    .Where(x => x.UserId == userID)
                    .List<OrgUnit>() as List<OrgUnit>;

                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ORGUNIT_BY_USERID, orgUnit, userID);

                return orgUnit.ToArray();
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
            using (ISession session = Session)
            {
                var components = session.QueryOver<Component>()
                    .Where(x => x.OrgUnit.EntityId == orgUnitGuid)
                    .List<Component>() as List<Component>;

                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_COMPONENT_BY_ORGUNIT_ID, components, orgUnitGuid);

                return components.ToArray();
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
            using (ISession session = Session)
            {
                var configs = session.QueryOver<OrgUnitConfig>().List<OrgUnitConfig>() as List<OrgUnitConfig>;

                return configs.ToArray();
            }
        }

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="orgUnit">The org unit.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        public Component CreateComponent(string componentName, OrgUnit orgUnit)
        {
            var component = new Component
            {
                Name = componentName,
                OrgUnit = orgUnit
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_COMPONENT, component);

            return component;
        }

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="content">the content of the result</param>
        /// <param name="url">The source.</param>
        /// <param name="title">The title.</param>
        /// <returns>
        /// the created result object
        /// </returns>
        public Result CreateResult(Component component, string content, string url, string title)
        {
            var result = new Result
            {
                Component = component,
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

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            DBManager.Initialize();
            base.StartService();
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
        /// Initializes this instance of DBManager.
        /// </summary>
        private static void Initialize()
        {
            if (SessionFactory == null)
            {
                ConfigureSession();
            }

            Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);
        }

        /// <summary>
        /// Configures the session.
        /// </summary>
        private static void ConfigureSession()
        {
            var configuration = new Configuration();

            configuration.Configure();
            configuration.AddAssembly(typeof(DBManager).Assembly);

            SessionFactory = configuration.BuildSessionFactory();
            new SchemaExport(configuration).Drop(false, false);

            Log.Debug(Properties.Resources.DBSESSION_NHIBERNATE_CONFIG_READY);
        }
    }
}