// ------------------------------------------------------------------------
// <copyright file="NutchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Extensions;
    using HSA.InfoSys.Common.Logging;
    using Newtonsoft.Json;
    using log4net;
    using WCFServices;

    /// <summary>
    /// This class invokes the crawl and handles all pending crawls.
    /// </summary>
    public class NutchController : Service
    {
        /// <summary>
        /// The thread logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchController");

        /// <summary>
        /// The nutch controller.
        /// </summary>
        private static NutchController nutchController;

        /// <summary>
        /// The db manager.
        /// </summary>
        private readonly DbManager dbManager;

        /// <summary>
        /// The crawlLock.
        /// </summary>
        private readonly object crawlLock = new object();

        /// <summary>
        /// The crawl process.
        /// </summary>
        private readonly IList<NutchControllerClient> nutchClients = new List<NutchControllerClient>();

        /// <summary>
        /// The nutch settings.
        /// </summary>
        private NutchControllerClientSettings settings;

        /// <summary>
        /// The URLs for crawling.
        /// </summary>
        private string[] urls = new string[0];

        /// <summary>
        /// Shows how mutch crawls are still running.
        /// </summary>
        private int runningCrawls;

        /// <summary>
        /// Indicates if ther is a crawl running or not.
        /// </summary>
        private bool isCrawling;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchController"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="dbManager">The db manager.</param>
        private NutchController(Guid serviceGUID, DbManager dbManager)
            : base(serviceGUID)
        {
            this.dbManager = dbManager;

            this.settings = dbManager.GetNutchClientSettings();
            this.URLs = dbManager.GetAllUrls();

            this.InitializeClients();
        }

        /// <summary>
        /// Our delegate for invoking an async crawl.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="dbManager">The db manager.</param>
        private delegate void InvokeCrawl(NutchControllerClientSettings settings, DbManager dbManager);

        /// <summary>
        /// Gets or sets the URLs.
        /// </summary>
        /// <value>
        /// The URLs.
        /// </value>
        private string[] URLs
        {
            get
            {
                return this.urls;
            }

            set
            {
                this.ServiceMutex.WaitOne();

                if (value != null && value.Length != 0)
                {
                    object tmp = value.Distinct().ToArray();

                    if (this.urls != tmp)
                    {
                        this.urls = tmp as string[];
                        Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_UPDATE_URLS, tmp);
                    }
                }

                this.ServiceMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Gets the nutch controller.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        /// <returns>
        /// A new nutch controller service.
        /// </returns>
        /// <value>
        /// The nutch factory.
        /// </value>
        public static NutchController NutchFactory(DbManager dbManager)
        {
            return nutchController ?? (nutchController = new NutchController(Guid.NewGuid(), dbManager));
        }

        /// <summary>
        /// Sets the next running crawl.
        /// </summary>
        private void SetNextCrawl()
        {
            lock (this.crawlLock)
            {
                try
                {
                    if (!this.isCrawling)
                    {
                        var newSettings = this.dbManager.GetNutchClientSettings();

                        if (newSettings.Equals(this.settings) == false)
                        {
                            Log.InfoFormat(
                                Properties.Resources.NUTCH_CONTROLLER_UPDATE_SETTINGS,
                                this.settings,
                                newSettings);

                            this.settings = newSettings;
                            this.InitializeClients();
                        }

                        if (this.settings.IsDefault() == false)
                        {
                            this.InitializeNextCrawl();

                            foreach (var client in this.nutchClients)
                            {
                                client.InitializeClient(this.settings);

                                if (client.IsClientUsable && client.URLs.Count > 0)
                                {
                                    Log.DebugFormat(
                                        Properties.Resources.NUTCH_CONTROLLER_SET_PENDING_CRAWL,
                                        client.Hostname,
                                        client.URLs.ElementsToString());

                                    var invokeCrawl = new InvokeCrawl(client.StartCrawl);

                                    var callback = new AsyncCallback(
                                        c =>
                                        {
                                            this.ServiceMutex.WaitOne();

                                            if (c.IsCompleted)
                                            {
                                                this.runningCrawls--;
                                                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CRAWL_FINISHED, this.runningCrawls);

                                                if (this.runningCrawls == 0)
                                                {
                                                    this.ResetCrawls();
                                                    this.isCrawling = false;

                                                    Log.Info(Properties.Resources.NUTCH_CONTROLLER_CRAWLS_FINISHED);
                                                }
                                            }

                                            this.ServiceMutex.ReleaseMutex();
                                        });

                                    this.runningCrawls++;
                                    invokeCrawl.BeginInvoke(this.settings, this.dbManager, callback, this);
                                }
                                else if (client.URLs.Count == 0)
                                {
                                    Log.WarnFormat(
                                        Properties.Resources.NUTCH_CONTROLLER_DO_NOT_CRAWL_ON_HOST_NO_URLS,
                                        client.Hostname,
                                        client);
                                }
                                else
                                {
                                    Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_DO_NOT_CRAWL_ON_HOST, client);
                                }
                            }

                            this.isCrawling = this.runningCrawls > 0;
                        }
                        else
                        {
                            //// No settings
                            Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_NO_SETTINGS);
                        }
                    }
                }
                catch (NoNutchClientUsableException nuce)
                {
                    Log.FatalFormat(Properties.Resources.NUTCH_CONTROLLER_NO_USABLE_CLIENT, nuce);
                    this.StopService(true);
                }
            }
        }

        /// <summary>
        /// Stops this instance, the running crawl and disconnects from the hosts.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            foreach (var client in this.nutchClients)
            {
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_DISCONNECT, client.Hostname);
                client.Disconnect();
            }

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this controller.
        /// </summary>
        protected override void Run()
        {
            while (this.Running)
            {
                if (!this.isCrawling && !this.Cancel)
                {
                    Log.Info(Properties.Resources.NUTCH_CONTROLLER_START_CRAWL);
                    this.SetNextCrawl();
                }

                Thread.Sleep(10000);

                Log.Info(Properties.Resources.NUTCH_CONTROLLER_IS_ALIVE);
            }
        }

        /// <summary>
        /// Initializes the clients.
        /// Initializes the clients also means to reinitialize
        /// them before the next crawl starts. This must be done
        /// because the settings may have changed.
        /// </summary>
        private void InitializeClients()
        {
            Log.Info(Properties.Resources.NUTCH_CONTROLLER_INITIALIZE_CLIENT);

            if (this.settings.IsDefault() == false)
            {
                this.nutchClients.Clear();

                var clients = new string[0];

                if (string.IsNullOrEmpty(this.settings.NutchClients) == false)
                {
                    clients = JsonConvert.DeserializeObject<string[]>(this.settings.NutchClients);

                    Log.DebugFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENTS_SPLIT,
                        clients.ElementsToString());
                }

                foreach (var client in clients)
                {
                    var nutchClient = new NutchControllerClient(this.settings, client);

                    Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_NEW_CLIENT_INITIALIZED, client);

                    this.nutchClients.Add(nutchClient);
                }

                Log.Info(Properties.Resources.NUTCH_CONTROLLER_INITIALIZE_CLIENT_FINISHED);
            }
            else
            {
                Log.Warn(Properties.Resources.NUTCH_CONTROLLER_NO_INITIALIZING_NO_SETTINGS);
            }
        }

        /// <summary>
        /// Initializes the next crawl.
        /// Prepares the clients for their next crawl.
        /// If a client is not reachable at this moment
        /// it will be excluded from this crawl. Next time
        /// if it is reachable again it can continue crawling.
        /// </summary>
        private void InitializeNextCrawl()
        {
            this.ServiceMutex.WaitOne();

            Log.Info(Properties.Resources.NUTCH_CONTROLLER_INIT_NEXT_CRAWL);

            int index = 0;

            foreach (var client in this.nutchClients)
            {
                Log.Info(Properties.Resources.NUTCH_CONTROLLER_CHECK_CLIENT_USAGE);
                client.CheckClientForUsage(this.settings);
            }

            this.URLs = this.dbManager.GetAllUrls();

            foreach (var url in this.URLs)
            {
                bool urlAdded = false;

                while (!urlAdded)
                {
                    var client = this.nutchClients[index % this.nutchClients.Count];

                    if (client.IsClientUsable)
                    {
                        client.URLs.Add(url);
                        urlAdded = true;
                        Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_ADD_URL, url, client.Hostname);
                    }
                    else
                    {
                        Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_NOT_USABLE, client.Hostname);
                    }

                    index++;

                    if (index % this.nutchClients.Count == 0 && !urlAdded)
                    {
                        throw new NoNutchClientUsableException(this.GetType().Name);
                    }
                }
            }

            foreach (var client in this.nutchClients)
            {
                if (client.IsClientUsable && client.URLs.Count > 0)
                {
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_SET_CRAWL_PROCESS, client.Hostname);
                    client.SetCrawlProcess(this.settings);
                }
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Resets the crawls.
        /// Clears the list of urls in each crawl
        /// client to avoid have duplicates.
        /// </summary>
        private void ResetCrawls()
        {
            this.ServiceMutex.WaitOne();

            foreach (var client in this.nutchClients)
            {
                if (client.URLs.Count > 0)
                {
                    client.URLs.Clear();
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_RESET_CLIENTS, client.Hostname);
                }
            }

            this.ServiceMutex.ReleaseMutex();
        }
    }
}
