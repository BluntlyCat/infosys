namespace HSA.InfoSys.WebCrawler
{

using System;
using System.Net;
using System.Threading;
using HSA.InfoSys.Logging;
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

        private ISolrOperations<SecurityIssues> solr;

        private void init()
        {
            //find the solr service
            Startup.Init<SecurityIssues>("http://infosys.informatik.hs-augsburg.de:8983/solr");

            solr = ServiceLocator.Current.GetInstance<ISolrOperations<SecurityIssues>>();
        }

        private void addSecurityIssues()
        {

            solr.Add(new SecurityIssues()
            {
               issue = 1,
               issueID = 1,
               issueDate = new System.DateTime(),
               threatLevel = 10,
               titel = "Windows Firewall Leak"

            });

            solr.Add(new SecurityIssues()
            {
               issue = 2,
               issueID = 2,
               issueDate = new System.DateTime(),
               threatLevel = 5,
               titel = "Linux Startup Leak"

            });

            solr.Add(new SecurityIssues()
            {
               issue = 3,
               issueID = 3,
               issueDate = new System.DateTime(),
               threatLevel = 3,
               titel = "DES Protokoll not safe anymore"

            });
            // commit to the index
            solr.Commit();  
           //</ISolrOperations<SecurityIssues></SecurityIssues></SecurityIssues> 
        }

        private void search()
        {
            Console.WriteLine("Linux Issues");
            var linuxIssues = solr.Query(new SolrQuery("linux"));

            foreach (SecurityIssues sI in linuxIssues)
            {
                Console.WriteLine(string.Format("{0}: {1}", sI.issueID , sI.issue, sI.titel));
            }
        }

        static void Main(string[] args)
        {
            Solr solrInstance = new Solr();
            solrInstance.init();
            solrInstance.addSecurityIssues();
            solrInstance.search();
        }
   }
}

