// ------------------------------------------------------------------------
// <copyright file="ServiceController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.CrawlerServiceController
{
    using System;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;

    /// <summary>
    /// This class provides control
    /// functionality for the crawler service.
    /// </summary>
    public class ServiceController
    {
        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var crawlerService = WCFControllerClient<ICrawlerService>.ClientProxy;

            switch (args[0])
            {
                case "--start":
                    crawlerService.StopServices();
                    break;

                case "--stop":
                    crawlerService.StopServices();
                    break;

                case "--shutdown":
                    crawlerService.ShutdownCrawler();
                    break;

                case "--help":
                default:
                    PrintHelp();
                    break;
            }
        }

        /// <summary>
        /// Prints the help.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("--start\tstarts the services.");
            Console.WriteLine("--stop\tstops the services.");
            Console.WriteLine("--shutdown\tshutdown the CrawlerService.");
            Console.WriteLine("--help\tprints this help text.");
            Console.WriteLine(string.Empty);
        }
    }
}
