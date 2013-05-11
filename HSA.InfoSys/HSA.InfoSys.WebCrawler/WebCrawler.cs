// ------------------------------------------------------------------------
// <copyright file="WebCrawler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using HSA.InfoSys.Logging;
    using log4net;

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    public class WebCrawler
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("WebCrawler");

        /// <summary>
        /// The running flag for this server.
        /// </summary>
        private static bool running;

        /// <summary>
        /// The controller for the crawler.
        /// </summary>
        private CrawlController controller;

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            WebCrawler crawler = new WebCrawler();
            crawler.RunServer();
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        private void RunServer()
        {
            DataTable factoryClasses = DbProviderFactories.GetFactoryClasses();

            Log.Debug("Starting server...");
            Log.Info("Press q for quit.");

            this.controller = new CrawlController();

            this.controller.StartServices();
            this.controller.OpenWCFHost();

#warning Only for testing, remove me when finished.
            this.controller.Test();

            running = true;

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    Log.InfoFormat("Got user input key {0}", keyInfo.Key);

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Log.Info("User exited the application.");
                        this.ShutdownCrawler();
                    }
                    else
                    {
                        Log.Info("Unkown user input.");
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void ShutdownCrawler()
        {
            if (this.controller != null)
            {
                this.controller.CloseWCFHost();
                this.controller.StopServices();

                running = false;
            }
        }
    }
}
