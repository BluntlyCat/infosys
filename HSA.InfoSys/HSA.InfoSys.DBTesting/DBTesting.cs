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
                        case ConsoleKey.A:
                            log.Info("Add entity to db.");

                            Guid guid;

                            var result = dbManager.CreateResult("Some Data...");
                            log.InfoFormat("Result Created: [{0}]", result.ToString());

                            var comp = dbManager.CreateComponent("Michis Special Component", "Funny Stuff");
                            log.InfoFormat("Component Created: [{0}]", comp.ToString());

                            guid = dbManager.AddEntity(comp);

                            var dbComp = dbManager.GetEntity<Component>(guid);
                            log.InfoFormat("Component from DB: [{0}]", dbComp);

                            dbComp.Result = result;
                            dbManager.UpdateEntity(dbComp);
                            log.InfoFormat("Component from DB updated: [{0}]", dbComp);

                            var dbComp2 = dbManager.GetEntity<Component>(guid, new Type[] { typeof(Result) });
                            log.InfoFormat("Component from DB: [{0}]", dbComp2);

                            var scheduler = dbManager.CreateScheduler(1, 12);
                            var config = dbManager.CreateSystemConfig("http://miitsoft.de", "michael@miitsoft.de", true, true, true, scheduler);
                            var service = dbManager.CreateSystemService(0, "Useless system", dbComp2, config);

                            guid = dbManager.AddEntity(service);

                            var dbService = dbManager.GetEntity<SystemService>(guid, new Type[] { typeof(Component), typeof(Result) });
                            log.InfoFormat("Service from DB: [{0}]", dbService);
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
                            log.Info("Send request to database.");

                          /*  string s = "99cee797-3ec0-458c-a016-a1bd0001bf34";
                            var newComp = dbManager.CreateComponent("Windows8", "TestWin");
                            dbManager.AddEntity(newComp);
                            var existingComp = dbManager.GetEntity<Component>(new Guid(s));*/
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
