﻿// ------------------------------------------------------------------------
// <copyright file="NutchTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.NutchTesting
{
    using System;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using System.Collections.Generic;

    /// <summary>
    /// Implement your testing methods for Nutch here.
    /// </summary>
    public class NutchTesting
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {

            //Test für das Hinzufügen von Urls und deren Filter in die jeweiligen Nutch verzeichznisse
            //
           // string regex = "C:/Users/A/Dropbox/Semester 6/Projekt/Tortoise/conf/";
           // string urls = "C:/Users/A/Dropbox/Semester 6/Projekt/Tortoise/urls/Nutch.txt";
           // NutchUrlManager m = new NutchUrlManager(regex, urls);
           // m.AddUrl("www.schokolade.de");


            ILog log = Logger<string>.GetLogger("NutchTesting");
            
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
                    log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.H:
                            log.Info("Print help text.");
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            log.Info("Quit application.");
                            running = false;
                            break;

                        case ConsoleKey.S:
                            log.Info("Send request to nutch.");
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
