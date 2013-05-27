// ------------------------------------------------------------------------
// <copyright file="DBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
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
        /// Gets the DB manager and ensures that the configuration
        /// will be executed only once and that there is only one db manager.
        /// </summary>
        /// <returns>
        /// The Database Manager
        /// </returns>
        public static IDBManager Manager
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
        /// <returns>An ISession to the session object.</returns>
        private static ISession Session
        {
            get
            {
                if (SessionFactory == null)
                {
                    var configuration = new Configuration();

                    configuration.Configure();
                    configuration.AddAssembly(typeof(DBManager).Assembly);

                    SessionFactory = configuration.BuildSessionFactory();
                    new SchemaExport(configuration).Drop(false, false);

                    Log.Debug(Properties.Resources.DBSESSION_NHIBERNATE_CONFIG_READY);
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
        public IList<OrgUnit> GetOrgUnitsByUserID(int userID)
        {
            using (ISession session = Session)
            using (ITransaction transaction = session.BeginTransaction())
            {
                var orgUnit = session.QueryOver<OrgUnit>()
                    .Where(x => x.UserId == userID)
                    .List<OrgUnit>();

                Log.InfoFormat(Properties.Resources.DBMANAGER_GET_ORGUNIT_BY_USERID, orgUnit, userID);

                return orgUnit;
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
        /// <param name="data">the content of the result</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// the created result object
        /// </returns>
        public Result CreateResult(string data, string source)
        {
            var result = new Result
            {
                Time = DateTime.Now,
                Source = source,
                Data = data
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
                Name = name,
                NextSearch = DateTime.Now
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
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <param name="scheduler">A scheduler object.</param>
        /// <returns>
        /// The created OrgUnitConfig object.
        /// </returns>
        public OrgUnitConfig CreateOrgUnitConfig(
            string urls,
            string emails,
            bool urlActive,
            bool emailNotification,
            bool schedulerActive,
            Scheduler scheduler)
        {
            var orgUnitConfig = new OrgUnitConfig
            {
                URLS = urls,
                Emails = emails,
                URLActive = urlActive,
                EmailActive = emailNotification,
                SchedulerActive = schedulerActive,
                Scheduler = scheduler
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_ORGUNITCONFIG, orgUnitConfig);

            return orgUnitConfig;
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
                Begin = DateTime.Now,
                Days = days,
                Hours = hours
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SCHEDULER, scheduler);

            return scheduler;
        }
    }
}