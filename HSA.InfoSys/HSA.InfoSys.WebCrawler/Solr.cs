namespace HSA.InfoSys.WebCrawler
{
    using System;
    using HSA.InfoSys.Logging;
    using log4net;
    using Microsoft.Practices.ServiceLocation;
    using SolrNet;

    /// <summary>
    /// Gets Connection to the Solr Server
    /// </summary>
    public class Solr
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("Solr");

        /// <summary>
        /// A specified solr operation.
        /// </summary>
        private ISolrOperations<SecurityIssues> solr;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void Init()
        {
            ////find the solr service
            Startup.Init<SecurityIssues>("http://infosys.informatik.hs-augsburg.de:8983/solr");

            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SecurityIssues>>();
        }

        /// <summary>
        /// Adds the security issues.
        /// </summary>
        private void AddSecurityIssues()
        {
            solr.Add(new SecurityIssues()
            {
                Issue = 1,
                IssueID = 1,
                IssueDate = new System.DateTime(),
                ThreatLevel = 10,
                Title = "Windows Firewall Leak"
            });

            solr.Add(new SecurityIssues()
            {
                Issue = 2,
                IssueID = 2,
                IssueDate = new System.DateTime(),
                ThreatLevel = 5,
                Title = "Linux Startup Leak"
            });

            solr.Add(new SecurityIssues()
            {
                Issue = 3,
                IssueID = 3,
                IssueDate = new System.DateTime(),
                ThreatLevel = 3,
                Title = "DES Protokoll not safe anymore"
            });

            ////commit to the index
            solr.Commit();
            ////</ISolrOperations<SecurityIssues></SecurityIssues></SecurityIssues> 
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        private void Search()
        {
            Log.Info("Linux Issues");

            var linuxIssues = solr.Query(new SolrQuery("linux"));

            foreach (SecurityIssues sI in linuxIssues)
            {
                Log.InfoFormat("{0}: {1}", sI.IssueID, sI.Issue, sI.Title);
            }
        }
    }
}