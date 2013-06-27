// ------------------------------------------------------------------------
// <copyright file="NutchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using System.Threading;

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
        /// Prevents a default instance of the <see cref="NutchController" /> class from being created.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="urls">The URLs.</param>
        private NutchController(Guid serviceGUID, string[] urls)
            : base(serviceGUID)
        {
            this.settings = DBManager.ManagerFactory(Guid.NewGuid()).GetSettingsFor<NutchControllerClientSettings>();

            this.URLs = urls;

            this.NutchFound = true;

            this.clients = this.settings.NutchClients.Split(',');

            foreach (var client in this.clients)
            {
                var nutchClient = new NutchControllerClient(this.settings, client);
                nutchClient.OnNutchNotFound += new NutchControllerClient.NutchNotFoundHandler(this.NutchClient_OnNutchNotFound);

                this.nutchClients.Add(nutchClient);
            }
        }

        /// <summary>
        /// Our delegate for invoking an async crawl.
        /// </summary>
        public delegate void InvokeCrawl();

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void CrawlFinishedHandler(object sender);

        /// <summary>
        /// Occurs when [on crawl finished].
        /// </summary>
        public event CrawlFinishedHandler OnCrawlFinished;

        /// <summary>
        /// Gets a value indicating whether [nutch found].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [nutch found]; otherwise, <c>false</c>.
        /// </value>
        public bool NutchFound { get; private set; }

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
                }

                this.ServiceMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Gets the nutch controller.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="urls">The URLs.</param>
        /// <returns>
        /// A new nutch controller service.
        /// </returns>
        /// <value>
        /// The nutch factory.
        /// </value>
        public static NutchController NutchFactory(Guid serviceGUID, params string[] urls)
        {
            if (nutchController == null)
            {
                nutchController = new NutchController(serviceGUID, urls);
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
                if (!this.isCrawling && this.NutchFound)
                {
                    this.InitializeNextCrawl();

                    foreach (var client in this.nutchClients)
                    {
                        if (client.URLs.Count > 0)
                        {
                            Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_SET_PENDING_CRAWL, client.URLs);

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
                                        }
                                    }

                                    this.ServiceMutex.ReleaseMutex();
                                });

                            this.crawlsFinished++;
                            invokeCrawl.BeginInvoke(callback, this);
                        }
                    }

                    this.isCrawling = true;
                }
            }
        }

        /// <summary>
        /// Occurs when [on nutch not found].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="nutchFound">if set to <c>true</c> [nutch found].</param>
        public void NutchClient_OnNutchNotFound(object sender, bool nutchFound)
        {
            //this.Running = false;
            //this.NutchFound = nutchFound;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running && this.NutchFound)
            {
                if (!this.isCrawling)
                {
                    this.SetNextCrawl();
                }

                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Initializes the next crawl.
        /// </summary>
        private void InitializeNextCrawl()
        {
            this.ServiceMutex.WaitOne();

            int index = 0;

            foreach (var url in this.URLs)
            {
                this.nutchClients[index % this.clients.Length].URLs.Add(url);
                index++;
            }

            foreach (var client in this.nutchClients)
            {
                if (client.URLs.Count > 0)
                {
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
                }
            }

            this.ServiceMutex.ReleaseMutex();
        }
    }
}
