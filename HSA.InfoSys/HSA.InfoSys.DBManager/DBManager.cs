// ------------------------------------------------------------------------
// <copyright file="DBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager
{
    using System;
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// The DBManager handles database requests.
    /// </summary>
    public class DBManager : IDBManager
    {
        /// <summary>
        /// The logger for db manager.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("DBManager");

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDBManager dbManager;

        /// <summary>
        /// The DB session factory.
        /// </summary>
        private static ISessionFactory sessionFactory;

        /// <summary>
        /// Prevents a default instance of the <see cref="DBManager"/> class from being created.
        /// </summary>
        private DBManager()
        {
        }

        /// <summary>
        /// Gets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    var configuration = new Configuration();

                    configuration.Configure();
                    configuration.AddAssembly(typeof(DBManager).Assembly);

                    sessionFactory = configuration.BuildSessionFactory();
                    new SchemaExport(configuration).Drop(false, false);
                }

                Log.Debug(Properties.Resources.DBSESSION_NHIBERNATE_CONFIG_READY);
                return sessionFactory;
            }
        }

        /// <summary>
        /// Gets the DB manager and ensures that the configuration
        /// will be executed only once and that there is only one db manager.
        /// </summary>
        /// <returns>
        /// The Database Manager
        /// </returns>
        public static IDBManager GetDBManager()
        {
            if (dbManager == null)
            {
                Log.Debug(Properties.Resources.DBMANAGER_NO_MANAGER_FOUND);
                dbManager = new DBManager();
            }

            return dbManager;
        }

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        /// <returns>The GUID of the added entity.</returns>
        public Guid AddEntity(Entity entity)
        {
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(entity);
                transaction.Commit();
                Log.Info(Properties.Resources.DBMANAGER_ADD_OBJECT);
            }

            return entity.EntityId;
        }

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entity);
                    transaction.Commit();
                    Log.Info(Properties.Resources.DBMANAGER_UPDATE_OBJECT);
                }

                return entity.EntityId;
            }
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <typeparam name="T">The type of what you want.</typeparam>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public T GetEntity<T>(Guid entityGUID, Type[] types = null) where T : Entity
        {
            T entity;
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                entity = session.Get<T>(entityGUID);

                if (types != null)
                {
                    entity.Unproxy(types);
                }
            }

            Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ENTITY, typeof(T), entity, entityGUID);

            return entity;
        }

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="componentCategory">The component category.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        public Component CreateComponent(string componentName, string componentCategory)
        {
            var component = new Component
            {
                Category = componentCategory,
                Name = componentName,
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_COMPONENT, component);

            return component;
        }

        /// <summary>
        /// Creates an source object.
        /// </summary>
        /// <param name="sourceURL">The URL where the source points to.</param>
        /// <returns>
        /// The created source object.
        /// </returns>
        public Source CreateSource(string sourceURL)
        {
            var source = new Source
            {
                URL = sourceURL
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE, source);

            return source;
        }

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">the content of the result</param>
        /// <returns>
        /// the created result object
        /// </returns>
        public Result CreateResult(string data)
        {
            var result = new Result
            {
                TimeStamp = DateTime.Now,
                Data = data
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE);

            return result;
        }

        /// <summary>
        /// Creates a SystemService object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="name">The system name.</param>
        /// <param name="component">A component object</param>
        /// <param name="sysconfig">A system config object</param>
        /// <returns>
        /// The created SystemService object
        /// </returns>
        public SystemService CreateSystemService(int userId, string name, Component component, SystemConfig sysconfig)
        {
            var systemService = new SystemService
            {
                UserId = userId,
                Name = name,
                TimeStamp = DateTime.Now,
                Component = component,
                SystemConfig = sysconfig
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE);

            return systemService;
        }

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
        public SystemConfig CreateSystemConfig(
            string url,
            string email,
            bool urlActive,
            bool emailNotification,
            bool schedulerActive,
            Scheduler scheduler)
        {
            var systemConfig = new SystemConfig
            {
                URL = url,
                Email = email,
                URLActive = urlActive,
                EmailNotification = emailNotification,
                SchedulerActive = schedulerActive,
                Scheduler = scheduler
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE);

            return systemConfig;
        }

        /// <summary>
        /// Creates a Scheduler object
        /// </summary>
        /// <param name="days">The days.</param>
        /// <param name="hours">The hours.</param>
        /// <returns>
        /// The created Scheduler object.
        /// </returns>
        public Scheduler CreateScheduler(int days, int hours)
        {
            var scheduler = new Scheduler
            {
                TimeStamp = DateTime.Now,
                Days = days,
                Hours = hours
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE);

            return scheduler;
        }

        /// <summary>
        /// Opens the session.
        /// </summary>
        /// <returns>An ISession to the session object.</returns>
        private static ISession OpenSession()
        {
            Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);
            return SessionFactory.OpenSession();
        }
    }
}