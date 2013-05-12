// ------------------------------------------------------------------------
// <copyright file="DBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.DBManager
{
    using System;
    using HSA.InfoSys.DBManager.Data;
    using HSA.InfoSys.Logging;
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
        /// Opens the session.
        /// </summary>
        /// <returns>An ISession to the session object.</returns>
        private static ISession OpenSession()
        {
            Log.Debug(Properties.Resources.DBSESSION_OPEN_SESSION);
            return SessionFactory.OpenSession();
        }

        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        public void AddNewObject(object entity)
        {
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(entity);
                transaction.Commit();
                Log.Info(Properties.Resources.DBMANAGER_ADD_OBJECT);
            }
        }

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        public void UpdateObject(object entity)
        {
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(entity);
                transaction.Commit();
                Log.Info(Properties.Resources.DBMANAGER_UPDATE_OBJECT);
            }
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <typeparam name="T">The type of what you want.</typeparam>
        /// <param name="entityGUID">The entity GUID.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public T GetEntity<T>(Guid entityGUID)
        {
            T entity;
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                entity = session.Get<T>(entityGUID);
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
                ComponentGUID = System.Guid.NewGuid(),
                Category = componentCategory,
                Name = componentName
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
                SourceGUID = System.Guid.NewGuid(),
                URL = sourceURL
            };

            Log.InfoFormat(Properties.Resources.DBMANAGER_CREATE_SOURCE, source);

            return source;
        }
    }
}