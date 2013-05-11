namespace HSA.InfoSys.DBManager
{
    using NHibernate;
    using NHibernate.Cfg;
    using log4net;
    using HSA.InfoSys.Logging;
    using System;

    /// <summary>
    /// Gets the db session by using the abstract factory pattern.
    /// </summary>
    public class DBSession
    {
        private static readonly ILog log = Logging.GetLogger("DBSession");

        private static ISessionFactory _sessionFactory;

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
                if (_sessionFactory == null)
                {
                    log.Debug("### Create Configure...");
                    var configuration = new Configuration();

                    log.Debug("### Configure...");
                    configuration.Configure();

                    log.Debug("### Add Assembly...");
                    configuration.AddAssembly(typeof(DBManager).Assembly);

                    log.Debug("### Build session factory...");
                    _sessionFactory = configuration.BuildSessionFactory();
                    log.Debug("### Sessionfactory built...");
                }

                log.Debug("### Return sessionfactory...");
                return _sessionFactory;
            }
        }

        /// <summary>
        /// Opens the session.
        /// </summary>
        /// <returns></returns>
        public static ISession OpenSession()
        {
            log.Debug("Open new session");
            return SessionFactory.OpenSession();   
        }
    }
}
