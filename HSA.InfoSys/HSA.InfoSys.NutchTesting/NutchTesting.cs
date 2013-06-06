// ------------------------------------------------------------------------
// <copyright file="NutchTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.NutchTesting
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Nutch;
    using log4net;

    /// <summary>
    /// Implement your testing methods for Nutch here.
    /// </summary>
    public class NutchTesting
    {
        /// <summary>
        /// The logger for NutchTesting.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchTesting");

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            INutchManager n = NutchManager.ManagerFactory;
            bool running = true;
           
            Console.WriteLine(string.Empty);
            Console.WriteLine("Here you can test the nutch funcionality.");
            Console.WriteLine("To see your options press h or press q for quit.");
            Console.WriteLine(string.Empty);

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    Log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.H:
                            Log.Info("Print help text.");
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            Log.Info("Quit application.");
                            running = false;
                            break;

                        case ConsoleKey.S:
                            
                            Log.Info("Send request to nutch.");
                            n.StartCrawl("path", 1, 1);
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
            Console.WriteLine("Press s to start a new request to nutch server.");
            Console.WriteLine(string.Empty);
        }
    }
}
