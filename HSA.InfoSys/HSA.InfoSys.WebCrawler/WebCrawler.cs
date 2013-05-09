namespace HSA.InfoSys.WebCrawler
{
    using System;
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
            Log.Debug("Starting server...");
            Log.Info("Press q for quit.");

            controller = new CrawlController();

            controller.StartServices();
            controller.OpenWCFHost();

            //todo: Only for testing, remove me when finished.
            controller.Test();
           
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
                        ShutdownCrawler();
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
            if (controller != null)
            {
                controller.CloseWCFHost();
                controller.StopServices();

                running = false;
            }
        }
    }
}
