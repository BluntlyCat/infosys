// ------------------------------------------------------------------------
// <copyright file="NutchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
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
        /// The pending crawls
        /// </summary>
        private Dictionary<Guid, Process> pendingCrawls;

        /// <summary>
        /// The running crawls
        /// </summary>
        private Dictionary<Guid, Process> runningCrawls;

        /// <summary>
        /// The new crawl job arrived.
        /// </summary>
        private bool newCrawlJobArrived = false;

        /// <summary>
        /// Prevents a default instance of the <see cref="NutchController"/> class from being created.
        /// </summary>
        private NutchController()
        {
            this.pendingCrawls = new Dictionary<Guid, Process>();
            this.runningCrawls = new Dictionary<Guid, Process>();
        }

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public delegate void CrawlFinishedHandler(object sender, Guid orgUnitGUID);

        /// <summary>
        /// Occurs when [on crawl finished].
        /// </summary>
        public event CrawlFinishedHandler OnCrawlFinished;

        /// <summary>
        /// Gets the nutch controller.
        /// </summary>
        /// <value>
        /// The nutch factory.
        /// </value>
        public static NutchController NutchFactory
        {
            get
            {
                if (nutchController == null)
                {
                    nutchController = new NutchController();
                }

                return nutchController;
            }
        }

        /// <summary>
        /// Sets the pending crawl.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The URLs.</param>
        public void SetPendingCrawl(Guid orgUnitGUID, string userName, int depth, int topN, params string[] urls)
        {
            this.ServiceMutex.WaitOne();

            var nutchClient = new NutchControllerClient();
            var process = nutchClient.GetCrawlProcess(userName, depth, topN, urls);

            this.pendingCrawls.Add(orgUnitGUID, process);
            this.newCrawlJobArrived = true;

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running)
            {
                this.ServiceMutex.WaitOne();

                if (this.newCrawlJobArrived)
                {
                    this.runningCrawls = this.pendingCrawls;
                    this.pendingCrawls = new Dictionary<Guid, Process>();
                    this.newCrawlJobArrived = false;
                }

                this.ServiceMutex.ReleaseMutex();

                foreach (var crawl in this.runningCrawls)
                {
                    crawl.Value.Start();
                    crawl.Value.WaitForExit();

                    if (this.OnCrawlFinished != null)
                    {
                        this.OnCrawlFinished(this, crawl.Key);
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
