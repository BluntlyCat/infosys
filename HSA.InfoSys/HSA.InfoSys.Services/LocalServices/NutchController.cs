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
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

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
        /// The crawl process.
        /// </summary>
        private IList<NutchControllerClient> nutchClients = new List<NutchControllerClient>();

        /// <summary>
        /// The settings.
        /// </summary>
        private NutchControllerClientSettings settings;

        /// <summary>
        /// The db manager.
        /// </summary>
        private DBManager dbManager;

        /// <summary>
        /// The lock mutex.
        /// </summary>
        private object lockMutex = new object();

        /// <summary>
        /// The URLs.
        /// </summary>
        private string[] urls = new string[0];

        /// <summary>
        /// The clients
        /// </summary>
        private string[] clients;

        /// <summary>
        /// The crawls finished.
        /// </summary>
        private int crawlsFinished = 0;

        /// <summary>
        /// The is crawling.
        /// </summary>
        private bool isCrawling = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchController"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="dbManager">The db manager.</param>
        private NutchController(Guid serviceGUID, DBManager dbManager)
            : base(serviceGUID)
        {
            this.dbManager = dbManager;

            this.settings = dbManager.GetSettingsFor<NutchControllerClientSettings>();

            this.URLs = dbManager.GetAllUrls();

            this.clients = this.settings.NutchClients.Split(',');

            foreach (var client in this.clients)
            {
                var nutchClient = new NutchControllerClient(this.settings, client);

                this.nutchClients.Add(nutchClient);
            }
        }

        /// <summary>
        /// Our delegate for invoking an async crawl.
        /// </summary>
        public delegate void InvokeCrawl();

        /// <summary>
        /// Gets or sets the URLs.
        /// </summary>
        /// <value>
        /// The URLs.
        /// </value>
        public string[] URLs
        {
            get
            {
                return this.urls;
            }

            set
            {
                this.ServiceMutex.WaitOne();

                var tmp = value.Distinct().ToArray();

                if (this.urls != tmp)
                {
                    this.urls = tmp;
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_UPDATE_URLS, tmp);
                }

                this.ServiceMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Gets the nutch controller.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="dbManager">The db manager.</param>
        /// <returns>
        /// A new nutch controller service.
        /// </returns>
        /// <value>
        /// The nutch factory.
        /// </value>
        public static NutchController NutchFactory(Guid serviceGUID, DBManager dbManager)
        {
            if (nutchController == null)
            {
                nutchController = new NutchController(serviceGUID, dbManager);
            }

            return nutchController;
        }

        /// <summary>
        /// Sets the pending crawl.
        /// </summary>
        public void SetNextCrawl()
        {
            lock (this.lockMutex)
            {
                try
                {
                    if (!this.isCrawling)
                    {
                        this.settings = dbManager.GetSettingsFor<NutchControllerClientSettings>();
                        this.InitializeNextCrawl();

                        foreach (var client in this.nutchClients)
                        {
                            if (client.IsClientUsable && client.URLs.Count > 0)
                            {
                                Log.DebugFormat(
                                    Properties.Resources.NUTCH_CONTROLLER_SET_PENDING_CRAWL,
                                    client.Hostname,
                                    client.URLs.ElementsToString());

                                InvokeCrawl invokeCrawl = new InvokeCrawl(client.StartCrawl);

                                AsyncCallback callback = new AsyncCallback(
                                    c =>
                                    {
                                        this.ServiceMutex.WaitOne();

                                        if (c.IsCompleted)
                                        {
                                            this.crawlsFinished--;

                                            if (this.crawlsFinished == 0)
                                            {
                                                this.ResetCrawls();
                                                this.isCrawling = false;

                                                Log.Info(Properties.Resources.NUTCH_CONTROLLER_CRAWLS_FINISHED);
                                            }
                                        }

                                        this.ServiceMutex.ReleaseMutex();
                                    });

                                this.crawlsFinished++;
                                invokeCrawl.BeginInvoke(callback, this);
                            }
                            else
                            {
                                Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_DO_NOT_CRAWL_ON_HOST, client.Hostname);
                            }
                        }

                        this.isCrawling = true;
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
        /// Stops this instance.
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
        /// Runs this instance.
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
        /// Initializes the next crawl.
        /// </summary>
        private void InitializeNextCrawl()
        {
            this.ServiceMutex.WaitOne();

            Log.Info(Properties.Resources.NUTCH_CONTROLLER_INIT_NEXT_CRAWL);

            int index = 0;

            foreach (var client in this.nutchClients)
            {
                Log.Info(Properties.Resources.NUTCH_CONTROLLER_CHECK_CLIENT_USAGE);
                client.CheckClientForUsage();
            }

            foreach (var url in this.URLs)
            {
                bool urlAdded = false;

                while (!urlAdded)
                {
                    var client = this.nutchClients[index % this.clients.Length];

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

                    if (index % this.clients.Length == 0 && !urlAdded)
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
                    client.SetCrawlProcess();
                }
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Resets the crawls.
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
