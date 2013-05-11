namespace HSA.InfoSys.DBManager
{
    using log4net;
    using HSA.InfoSys.Logging;
    using PetaPoco;
    using System.Data;
    using System.Data.Common;
    using System.Collections.Generic;

    /// <summary>
    /// Gets the db session by using the abstract factory pattern.
    /// </summary>
    public class DBSession
    {
        private static readonly ILog log = Logging.GetLogger("DBSession");

        private static Database _dataBase;

        /// <summary>
        /// Gets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        private static Database Database
        {
            get
            {
                if (_dataBase == null)
                {
                    _dataBase = new Database("mysql");
                }

                log.Debug("### Return sessionfactory...");
                return _dataBase;
            }
        }

        public static IEnumerable<T> Query<T>(string query)
        {
            return Database.Query<T>(query);
        }

        public static void Add(object entity)
        {
            Database.Insert(entity);
        }

        public static void Update(object entity)
        {
            Database.Update("Component", "componentGUID", entity);
        }
    }
}
