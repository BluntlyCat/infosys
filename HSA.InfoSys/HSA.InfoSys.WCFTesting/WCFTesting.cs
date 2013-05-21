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
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
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
            ILog log = Logging.GetLogger("WCFTesting");

            ICrawlController controller;

            bool running = true;

            bool requestSent = false;

            string response = string.Empty;

            Console.WriteLine(string.Empty);
            Console.WriteLine("Here you can test the WCF feautures to the WebCrawler.");
            Console.WriteLine("To see your options press h or press q for quit.");
            Console.WriteLine(string.Empty);

            while (running)
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
                            controller = CrawlController.ClientProxy;
                            log.Info("Add new Component.");

                            var result = new Result();
                            result.Data = "Some data";
                            result.EntityId = Guid.NewGuid();
                            result.TimeStamp = new DateTime();

                            var comp = controller.CreateComponent("Michis Special Component", "Funny Stuff", result) as Component;
                            log.InfoFormat("Component Created: [{0}]", comp.ToString());
                            Guid cguid = controller.AddEntity(comp);
                            var dbComp = controller.GetEntity(cguid) as Component;
                            log.InfoFormat("Component from DB: [{0}]", dbComp);

                            var source = controller.CreateSource("http://miitsoft.de") as Source;
                            log.InfoFormat("Source Created: [{0}]", source.ToString());
                            Guid sguid = controller.AddEntity(source);
                            var dbSrc = controller.GetEntity(sguid) as Source;
                            log.InfoFormat("Source from DB: [{0}]", dbSrc);
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
                                controller = CrawlController.ClientProxy;
                                controller.StartSearch("solr");
                            }
                            catch (Exception e)
                            {
                                log.ErrorFormat("Unable to communicate with host: [{0}]", e);
                            }

                            break;
                    }
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
