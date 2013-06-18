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
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using HSA.InfoSys.Common.Entities;

    /// <summary>
    /// Implement your testing methods for WCF here.
    /// </summary>
    public class WCFTesting
    {
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
                                /*var org1 = WCFControllerClient<IDBManager>.ClientProxy.CreateOrgUnit(32, "Desktop PC");

                                var orgId = WCFControllerClient<IDBManager>.ClientProxy.AddEntity(org1);
                                org1 = WCFControllerClient<IDBManager>.ClientProxy.GetEntity(orgId) as OrgUnit;

                                var comp1 = WCFControllerClient<IDBManager>.ClientProxy.CreateComponent("Windows", org1);
                                var comp2 = WCFControllerClient<IDBManager>.ClientProxy.CreateComponent("Solr", org1);
                                var comp3 = WCFControllerClient<IDBManager>.ClientProxy.CreateComponent("Office", org1);
                                var comp4 = WCFControllerClient<IDBManager>.ClientProxy.CreateComponent("Star Money", org1);

                                WCFControllerClient<IDBManager>.ClientProxy.AddEntitys(comp1, comp2, comp3, comp4);*/

                                var orgUnitGuids = WCFControllerClient<IDBManager>.ClientProxy.GetOrgUnitsByUserID(32).ToList<OrgUnit>();
                                WCFControllerClient<ISolrController>.ClientProxy.SearchForOrgUnit(orgUnitGuids.First().EntityId);

                                break;

                            case ConsoleKey.G:
                                var c = WCFControllerClient<IDBManager>.ClientProxy;
                                var entity = c.GetEntity(new Guid("23c83f7f-a371-43ad-8734-a1c8013b55ee"));

                                Log.InfoFormat("Entity: [{0}]", entity);
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

                                try
                                {
                                    Log.Info("Got client proxy...");
                                    var orgUnitGuids2 = WCFControllerClient<IDBManager>.ClientProxy.GetOrgUnitsByUserID(32).ToList<OrgUnit>();

                                    WCFControllerClient<ISolrController>.ClientProxy.SearchForComponent(orgUnitGuids2.First().EntityId);
                                }
                                catch (Exception e)
                                {
                                    Log.ErrorFormat("Unable to communicate with host: [{0}]", e);
                                }

                                break;

                            case ConsoleKey.T:
                                var OrgUconf = WCFControllerClient<IDBManager>.ClientProxy.CreateOrgUnitConfig("miitsoft.de", "michael@miitsoft.de", true, true, 2, 10, new DateTime(), true);
                                Log.DebugFormat("Config: {0}", OrgUconf);

                                var orgU = WCFControllerClient<IDBManager>.ClientProxy.CreateOrgUnit(32, "Webserver");
                                orgU.OrgUnitConfig = OrgUconf;
                                Log.DebugFormat("OrgUnit: {0}", orgU);

                                var comp = WCFControllerClient<IDBManager>.ClientProxy.CreateComponent("Apache", orgU);
                                Log.DebugFormat("Component: {0}", comp);

                                var result = WCFControllerClient<IDBManager>.ClientProxy.CreateResult(comp, "content", "url", "problem");
                                result.Component = comp;
                                Log.DebugFormat("Result: {0}", result);

                                var comps = WCFControllerClient<IDBManager>.ClientProxy.GetComponentsByOrgUnitId(Guid.Empty).ToList<Component>();
                                Log.DebugFormat("Components: {0}", comps);

                                var orgUnits = WCFControllerClient<IDBManager>.ClientProxy.GetOrgUnitsByUserID(32).ToList<OrgUnit>();
                                Log.DebugFormat("OrgUnits: {0}", orgUnits);

                                var configs = WCFControllerClient<IDBManager>.ClientProxy.GetOrgUnitConfigurations().ToList<OrgUnitConfig>();
                                Log.DebugFormat("Configurations: {0}", configs);
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
