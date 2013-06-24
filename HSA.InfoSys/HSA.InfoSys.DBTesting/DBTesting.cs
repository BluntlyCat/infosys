// ------------------------------------------------------------------------
// <copyright file="DBTesting.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Testing.DBTesting
{
    using System;
    using System.Linq;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
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
            ILog log = Logger<string>.GetLogger("WCFTesting");

            IDBManager dbManager = DBManager.ManagerFactory(Guid.NewGuid());
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
                            var orgUnit = dbManager.CreateOrgUnit(0, "Webserver");
                            var component = dbManager.CreateComponent("Apache", orgUnit.EntityId);
                            guid = dbManager.AddEntity(component);

                            var component2 = dbManager.GetEntity(guid, dbManager.LoadThisEntities("OrgUnit"));
                            log.InfoFormat("Got component from db: [{0}]", component2);

                            break;

                        case ConsoleKey.D:
                            var dguid = new Guid("8f662bf9-4118-48c2-8a89-a1c8007cfd86");
                            var entity = dbManager.GetEntity(dguid);
                            dbManager.DeleteEntity(entity);
                            break;

                        case ConsoleKey.G:
                            var e = dbManager.GetEntity(new Guid("01b5e81b-fa58-47a5-b4af-a1c8013b55ff"));
                            log.InfoFormat("Entity: [{0}]", e);
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

                        case ConsoleKey.T:
                            var entities = dbManager.GetOrgUnitsByUserID(32).ToList<OrgUnit>();
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
            Console.WriteLine("Press a to add an entity.");
            Console.WriteLine("Press d to delete an entity.");
            Console.WriteLine("Press h to see this help text.");
            Console.WriteLine("Press q to quit this application.");
            Console.WriteLine("Press s to start a new request to db server.");
            Console.WriteLine("Press t to test some new method.");

            Console.WriteLine(string.Empty);
        }
    }
}
