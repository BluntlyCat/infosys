namespace HSA.InfoSys.DBManager
{
    using HSA.InfoSys.Logging;
    using log4net;
    using NHibernate;
    using NHibernate.Cfg;

    /// <summary>
    /// Gets the db session by using the abstract factory pattern.
    /// </summary>
    public class DBSession
    {
        /// <summary>
        /// The logger of DBSession.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("DBSession");

        /// <summary>
        /// The DB session factory.
        /// </summary>
        private static ISessionFactory sessionFactory;

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
                }

                Log.Debug("NHibernate successfully configured and session factory ready.");
                return sessionFactory;
            }
        }

        /// <summary>
        /// Opens the session.
        /// </summary>
        /// <returns>An ISession to the session object.</returns>
        public static ISession OpenSession()
        {
            Log.Debug("Open new session");
            return SessionFactory.OpenSession();   
        }
    }
}
