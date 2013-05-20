// ------------------------------------------------------------------------
// <copyright file="DBTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.DBTesting
{
    using System;
    using System.Threading;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// Implement your testing methods for NHibernate here.
    /// </summary>
    public class DBTesting
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            ILog log = Logging.GetLogger("WCFTesting");

            IDBManager dbManager = DBManager.GetDBManager();

            bool running = true;

            Console.WriteLine(string.Empty);
            Console.WriteLine("Here you can test the DB Nhibernate feautures to the DB Server.");
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
                            log.Info("Send request to database.");

                            string s = "99cee797-3ec0-458c-a016-a1bd0001bf34";
                            var newComp = dbManager.CreateComponent("Windows8", "TestWin");
                            dbManager.AddEntity(newComp);
                            var existingComp = dbManager.GetEntity<Component>(new Guid(s));
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
            Console.WriteLine("Press s to start a new request to db server.");

            Console.WriteLine(string.Empty);
        }
    }
}
