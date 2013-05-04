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

    public class DBManager:IDBManager
    {

        private ISessionFactory sessionFactory;
        private Configuration configuration;

        /// <summary>
        /// Constructor. creates a configuration Object and add the DBManger as
        /// Assembly
        /// has also the DB-Schema of Nhibernate
        /// </summary>
        public  DBManager()
        {
            configuration = new Configuration();
            configuration.Configure();
            configuration.AddAssembly(typeof(DBManager).Assembly);
            sessionFactory = configuration.BuildSessionFactory();
            new SchemaExport(configuration).Drop(false, false);
            //new SchemaExport(configuration).Create(true, true);
        }

        /// <summary>
        /// adds a new Object (Component, Issue, Source)
        /// and saves it in database
        /// </summary>
        /// <param name="obj">Object</param>
        public void addNewObject(object obj)
        {
            using (ISession session = DBSession.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(obj);
                    transaction.Commit();
                    Console.WriteLine("Instance saved successfully in database");
                }
            }
        }

        /// <summary>
        /// saves changings of a object in database
        /// </summary>
        /// <param name="obj">Object</param>
        public void updateObject(object obj)
        {
            using (ISession session = DBSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Update(obj);
                transaction.Commit();
                Console.WriteLine("Instance updated successfully in database");
            }
        }

        /// <summary>
        /// returns a Component-Object 
        /// </summary>
        /// <param name="componentGUID">Id of the Object</param>
        /// <returns>Component-Object</returns>
        public Component getComponent(Guid componentGUID)
        {
            Component component;
            using (ISession session = DBSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                component = session.Get<Component>(componentGUID);
            }
            return component;  
        }

        /// <summary>
        /// return a Issue-Object from database
        /// </summary>
        /// <param name="issueGUID">Id of the Object </param>
        /// <returns>Issue-Object</returns>
        public Issue getIssue(Guid issueGUID)
        {
            Issue issue;
            using (ISession session = DBSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                issue = session.Get<Issue>(issueGUID);
            }
            return issue;
        }

        /// <summary>
        /// returns a Source-Object from database
        /// </summary>
        /// <param name="sourceGUID">Id of the Object</param>
        /// <returns>Source-Object</returns>
        public Source getSource(Guid sourceGUID)
        {
            Source source;
            using (ISession session = DBSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                source = session.Get<Source>(sourceGUID);
            }
            return source;  
        }

        /// <summary>
        /// creates an Component-Object and returns it
        /// </summary>
        /// <param name="componentName">name of the component</param>
        /// <param name="componentCategory">categroy of the component</param>
        /// <returns>Component-Object</returns>
        public Component createComponent(string componentName, string componentCategory)
        {
            var component = new Component { componentGUID = System.Guid.NewGuid(), category = componentCategory, name = componentName };
            return component;
        }

        /// <summary>
        /// creates an Source-Object and returns it
        /// </summary>
        /// <param name="sourceURL">URL</param>
        /// <returns> Source-Object</returns>
        public Source createSource(string sourceURL)
        {
            var source = new Source { sourceGUID = System.Guid.NewGuid(), URL = sourceURL };
            return source;
        }
    }
}