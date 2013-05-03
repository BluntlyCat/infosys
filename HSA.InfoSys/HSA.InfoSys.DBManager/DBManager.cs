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

        public  DBManager()
        {

            configuration = new Configuration();
            configuration.Configure();
            configuration.AddAssembly(typeof(DBManager).Assembly);
            sessionFactory = configuration.BuildSessionFactory();
            new SchemaExport(configuration).Drop(false, false);
            //new SchemaExport(configuration).Create(true, true);
        }

        public void addNewObject(object obj)
        {
            using (ISession session = DBSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(obj);
                transaction.Commit();
                Console.WriteLine("Instance saved successfully in database");
            }
        }


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
    }
}