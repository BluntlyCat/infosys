// ------------------------------------------------------------------------
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
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    /// <summary>
    /// Implement your testing methods for WCF here.
    /// </summary>
    public class WCFTesting
    {
        /// <summary>
        /// The logger for WCFTesting
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("WCFTesting");

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
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
                        Log.InfoFormat("Got response from solr: [{0}]", response);

                        requestSent = false;
                    }

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        Log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.A:
                                break;

                            case ConsoleKey.G:
                                break;

                            case ConsoleKey.H:
                                Log.Info("Print help text.");
                                PrintHelp();
                                break;

                            case ConsoleKey.Q:
                                Log.Info("Quit application.");
                                running = false;
                                break;

                            case ConsoleKey.S:
                                Log.Info("Send request to host.");
                                break;

                            case ConsoleKey.T:
                                break;

                            case ConsoleKey.U:
                                break;
                        }
                    }
                }
                catch (QuotaExceededException e)
                {
                    Log.ErrorFormat("Quota Exceeded: {0}", e);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Message: {0}", e);
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
