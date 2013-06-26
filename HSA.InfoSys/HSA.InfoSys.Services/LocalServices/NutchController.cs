// ------------------------------------------------------------------------
// <copyright file="NutchController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
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
        /// The URLs.
        /// </summary>
        private string[] urls = new string[0];

        /// <summary>
        /// Prevents a default instance of the <see cref="NutchController" /> class from being created.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="urls">The URLs.</param>
        private NutchController(Guid serviceGUID, string[] urls)
            : base(serviceGUID)
        {
            this.URLs = urls;
            this.NutchFound = true;
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

                if (this.urls != value)
                {
                    this.urls = value;
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
        public static NutchController NutchFactory(Guid serviceGUID, string[] urls)
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
                if (!this.Running && this.NutchFound)
                {
                    this.ServiceMutex.WaitOne();

                    var nutchClient = new NutchControllerClient();
                    this.crawlProcess = nutchClient.CreateCrawlProcess(this.URLs);

                    Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_SET_PENDING_CRAWL, this.URLs);

                    this.ServiceMutex.ReleaseMutex();

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
            }
            catch (Win32Exception w32e)
            {
                Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_NUTCH_NOT_FOUND, w32e);
                this.NutchFound = false;
            }
            catch (Exception e)
            {
                Log.DebugFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
            finally
            {
                this.Running = false;
            }
        }
    }
}
