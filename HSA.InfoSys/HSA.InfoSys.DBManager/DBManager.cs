namespace HSA.InfoSys.DBManager
{
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Logging;
    using log4net;
    using HSA.InfoSys.DBManager.Data;

    /// <summary>
    /// The DBManager handles database requests.
    /// </summary>
    public class DBManager
    {
        private static readonly ILog log = Logging.GetLogger("DBManager");

        private ISession session;

        public ISession Session
        {
            get
            {
                return session;
            }

            private set
            {
                if (session == null)
                {
                    this.session = value;
                }
            }
        }

        public DBManager()
        {
            try
            {
                Configuration config = new Configuration();
                config.Configure();
                config.AddAssembly(typeof(DBManager).Assembly);

                ISessionFactory sessionFactory = config.BuildSessionFactory();

                var schema = new SchemaExport(config);

                schema.Create(true, true);

                Session = sessionFactory.OpenSession();

               // Session.QueryOver<Component>().Where(x => x.name == "Webserver");
                
            }
            catch (Exception ex)
            {
                Exception e = ex;
                log.Error(e.Message);

                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    log.Error(e.Message);
                }
            }
        }
    }
}
