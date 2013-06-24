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
        /// The crawl process.
        /// </summary>
        private Process crawlProcess;

        /// <summary>
        /// The lock mutex.
        /// </summary>
        private object lockMutex = new object();

        /// <summary>
        /// Prevents a default instance of the <see cref="NutchController" /> class from being created.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private NutchController(Guid serviceGUID) : base(serviceGUID)
        {
        }

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
        /// Gets the nutch controller.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <returns>A new nutch controller service.</returns>
        /// <value>
        /// The nutch factory.
        /// </value>
        public static NutchController NutchFactory(Guid serviceGUID)
        {
            if (nutchController == null)
            {
                nutchController = new NutchController(serviceGUID);
            }

            return nutchController;
        }

        /// <summary>
        /// Sets the pending crawl.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The URLs.</param>
        public void SetNextCrawl(string folder, int depth, int topN, params string[] urls)
        {
            lock (this.lockMutex)
            {
                if (!this.Running)
                {
                    var nutchClient = new NutchControllerClient();
                    this.crawlProcess = nutchClient.CreateCrawlProcess(folder, depth, topN, urls);

                    Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_SET_PENDING_CRAWL, urls);

                    this.StartService();
                }
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            try
            {
                this.crawlProcess.Start();
                this.crawlProcess.WaitForExit();
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CRAWL_FINISHED, this.crawlProcess.StartInfo.Arguments);

                if (this.OnCrawlFinished != null)
                {
                    this.OnCrawlFinished(this);
                }

                this.Running = false;
            }
            catch (Exception e)
            {
                Log.DebugFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }
    }
}
