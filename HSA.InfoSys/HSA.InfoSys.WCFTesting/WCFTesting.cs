// ------------------------------------------------------------------------
// <copyright file="WCFTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.WCFTesting
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services;
    using HSA.InfoSys.Common.Services.Data;
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
                                ////var result = CrawlControllerClient<IDBManager>.ClientProxy.CreateResult("Schmeckt gut!", "Selbst getestet.");
                                ////var putensalami = CrawlControllerClient<IDBManager>.ClientProxy.GetEntity(new Guid("23c83f7f-a371-43ad-8734-a1c8013b55ee")) as Component;
                                ////putensalami.Result = result;
                                ////CrawlControllerClient<IDBManager>.ClientProxy.UpdateEntity(putensalami);

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
                                    CrawlControllerClient<ISolrController>.ClientProxy.StartSearch("solr");
                                }
                                catch (Exception e)
                                {
                                    log.ErrorFormat("Unable to communicate with host: [{0}]", e);
                                }

                                break;

                            case ConsoleKey.T:
                                var config = new OrgUnitConfig();
                                config.SchedulerActive = true;
                                config.Time = 2;
                                config.Days = 1;

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
