﻿// ------------------------------------------------------------------------
// <copyright file="WCFTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.WCFTesting
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services;
    using log4net;

    /// <summary>
    /// Implement your testing methods for WCF here.
    /// </summary>
    public class WCFTesting
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            ILog log = Logger<string>.GetLogger("WCFTesting");

            IDBManager dbManager = DBManager.ManagerFactory;

            bool running = true;

            bool requestSent = false;

            string response = string.Empty;

            Console.WriteLine(string.Empty);
            Console.WriteLine("Here you can test the WCF feautures to the WebCrawler.");
            Console.WriteLine("To see your options press h or press q for quit.");
            Console.WriteLine(string.Empty);

            while (running)
            {
                try
                {
                    if (!string.IsNullOrEmpty(response) && requestSent)
                    {
                        log.InfoFormat("Got response from solr: [{0}]", response);

                        requestSent = false;
                    }

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.A:
                                /*var org1 = CrawlControllerClient<IDBManager>.ClientProxy.CreateOrgUnit(32, "Desktop PC");

                                var orgId = CrawlControllerClient<IDBManager>.ClientProxy.AddEntity(org1);
                                org1 = CrawlControllerClient<IDBManager>.ClientProxy.GetEntity(orgId) as OrgUnit;

                                var comp1 = CrawlControllerClient<IDBManager>.ClientProxy.CreateComponent("Windows", org1);
                                var comp2 = CrawlControllerClient<IDBManager>.ClientProxy.CreateComponent("Solr", org1);
                                var comp3 = CrawlControllerClient<IDBManager>.ClientProxy.CreateComponent("Office", org1);
                                var comp4 = CrawlControllerClient<IDBManager>.ClientProxy.CreateComponent("Star Money", org1);

                                CrawlControllerClient<IDBManager>.ClientProxy.AddEntitys(comp1, comp2, comp3, comp4);*/

                                var orgUnitGuids = CrawlControllerClient<IDBManager>.ClientProxy.GetOrgUnitsByUserID(32);
                                CrawlControllerClient<ISolrController>.ClientProxy.SearchForOrgUnit(orgUnitGuids.First().EntityId);

                                break;

                            case ConsoleKey.G:
                                var c = CrawlControllerClient<IDBManager>.ClientProxy;
                                var entity = c.GetEntity(new Guid("23c83f7f-a371-43ad-8734-a1c8013b55ee"));

                                log.InfoFormat("Entity: [{0}]", entity);
                                break;

                            case ConsoleKey.H:
                                log.Info("Print help text.");
                                PrintHelp();
                                break;

                            case ConsoleKey.Q:
                                log.Info("Quit application.");
                                running = false;
                                break;

                            case ConsoleKey.S:
                                log.Info("Send request to host.");

                                try
                                {
                                    log.Info("Got client proxy...");
                                    var orgUnitGuids2 = CrawlControllerClient<IDBManager>.ClientProxy.GetOrgUnitsByUserID(32);

                                    CrawlControllerClient<ISolrController>.ClientProxy.SearchForComponent(orgUnitGuids2.First().EntityId);
                                }
                                catch (Exception e)
                                {
                                    log.ErrorFormat("Unable to communicate with host: [{0}]", e);
                                }

                                break;

                            case ConsoleKey.T:
                                var config = dbManager.CreateOrgUnitConfig(
                                    string.Empty,
                                    string.Empty,
                                    true,
                                    true,
                                    1,
                                    2,
                                    new DateTime(),
                                    true);

                                dbManager.AddEntity(config);

                                CrawlControllerClient<IScheduler>.ClientProxy.AddOrgUnitConfig(config);
                                break;

                            case ConsoleKey.U:
                                break;
                        }
                    }
                }
                catch (QuotaExceededException e)
                {
                    log.ErrorFormat("Quota Exceeded: {0}", e);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Message: {0}", e);
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Prints the help.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Press h to see this help text.");
            Console.WriteLine("Press q to quit this application.");
            Console.WriteLine("Press s to start a new search request to solr server.");

            Console.WriteLine(string.Empty);
        }
    }
}
