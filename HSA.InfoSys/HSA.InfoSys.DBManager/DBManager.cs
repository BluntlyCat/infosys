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
    public class DBManager : IDBManager
    {
        private static readonly ILog log = Logging.GetLogger("DBManager");
        private ISessionFactory sessionFactory;
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
                sessionFactory = config.BuildSessionFactory();

                var schema = new SchemaExport(config);
                //schema.Create(true, true);
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

        private void insert(Object obj)
        {
            try
            {
                using (Session = sessionFactory.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(obj);
                        transaction.Commit();
                    }
                    Console.WriteLine("Saved to the database");
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// creates a new Issue Object
        /// </summary>
        /// <param name="issueGUID">issueGUID</param>
        /// <param name="issueId">issueId</param>
        /// <param name="text">issuetext</param>
        /// <param name="titel">issue titel</param>
        /// <param name="threatLevel">issue threatlevel</param>
        /// <param name="date">issue date</param>
        public void addNewIssue(Guid new_issueGUID, int new_issueId, string new_text, string new_titel, int new_threatLevel, DateTime new_date)
        {
            Issue issue = new Issue { issueGUID = new_issueGUID, issueId = new_issueId, text = new_text, titel = new_titel, threatLevel = new_threatLevel, date = new_date };
            this.insert(issue);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="new_componentGUID"></param>
        /// <param name="new_componentId"></param>
        /// <param name="new_category"></param>
        /// <param name="new_name"></param>
        public void addNewComponent(Guid new_componentGUID, int new_componentId, string new_category, string new_name)
        {
            Component component = new Component { componentGUID = new_componentGUID, category = new_category, name = new_name, componentId = new_componentId };
            this.insert(component);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="new_sourceGUID"></param>
        /// <param name="new_sourceId"></param>
        /// <param name="new_URL"></param>
        public void addNewSource(Guid new_sourceGUID, int new_sourceId, string new_URL)
        {
            Source source = new Source { sourceGUID = new_sourceGUID, sourceId = new_sourceId, URL = new_URL };
            this.insert(source);
        }

     

        public void query()
        {
            throw new NotImplementedException();
        }
    }
}
