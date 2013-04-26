namespace HSA.InfoSys.WebCrawler
{

using System;
using System.Net;
using System.Threading;
using HSA.InfoSys.Logging;
using HSA.InfoSys.DBManager;
using log4net;
using SolrNet;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using Microsoft.Practices.ServiceLocation;  



    /// <summary>
    /// Gets Connection to the Solr Server
    /// </summary>
    class Solr
    {
        private static readonly ILog log = Logging.GetLogger("Solr");
        /// <summary>
        /// Constructor for Initialisation
        /// </summary>
        public Solr()
        {
            Startup.Init<Product>("http://infosys.informatik.hs-augsburg.de:8983/solr");

            NHibernate.Cfg.Configuration cfg = SetupNHibernate();
            var cfgHelper = new NHibernate.SolrNet.CfgHelper();
            cfgHelper.Configure(cfg, true); // true -> autocommit Solr after every operation (not really recommended)

        }
    }
}
