namespace HSA.InfoSys.DBManager
{
    using log4net;
    using HSA.InfoSys.Logging;
    using PetaPoco;
    using System.Data;
    using System.Data.Common;
    using System.Collections.Generic;
    using System;

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
        public static Database Database
        {
            get
            {
                if (_dataBase == null)
                {
                    Database = new Database("mysql");
                }

                log.Debug("### Return sessionfactory...");
                return _dataBase;
            }

            private set
            {
                _dataBase = value;
            }
        }
    }
}
