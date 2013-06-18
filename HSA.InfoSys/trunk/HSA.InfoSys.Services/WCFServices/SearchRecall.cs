﻿// ------------------------------------------------------------------------
// <copyright file="SearchRecall.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class implements the method for
    /// indicating that a search request for 
    /// an org unit is finished.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SearchRecall : Service, ISearchRecall
    {
        private static readonly ILog Log = Logger<string>.GetLogger("SearchRecall");

        /// <summary>
        /// The search recall
        /// </summary>
        private static SearchRecall searchRecall;

        /// <summary>
        /// The amount of running searches.
        /// </summary>
        private int runningSearches = 0;

        /// <summary>
        /// The timeout by default 10 minutes.
        /// </summary>
        private int timeout = 600000;

        /// <summary>
        /// Prevents a default instance of the <see cref="SearchRecall"/> class from being created.
        /// </summary>
        private SearchRecall()
        {
            this.Searches = new Dictionary<Guid, bool>();
        }

        /// <summary>
        /// The recall handler indicates that all search requests are finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void RecallHandler(object sender, Guid orgUnitGuid);

        /// <summary>
        /// Occurs when [recall].
        /// </summary>
        public event RecallHandler OnRecall;

        /// <summary>
        /// Gets the search recall.
        /// </summary>
        /// <value>
        /// The search recall.
        /// </value>
        public static SearchRecall SearchRecallFactory
        {
            get
            {
                if (searchRecall == null)
                {
                    searchRecall = new SearchRecall();
                }

                return searchRecall;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                if (!this.Running && value > 0 && this.timeout != value)
                {
                    this.timeout = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return this.Running;
            }
        }

        /// <summary>
        /// Gets the amount of running searches.
        /// </summary>
        /// <value>
        /// The amount of running searches.
        /// </value>
        public int RunningSearches
        {
            get
            {
                return this.runningSearches;
            }
        }

        /// <summary>
        /// Gets the searches.
        /// </summary>
        /// <value>
        /// The searches.
        /// </value>
        public Dictionary<Guid, bool> Searches { get; private set; }

        /// <summary>
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void Recall(Guid orgUnitGUID)
        {
            this.ServiceMutex.WaitOne();

            Log.DebugFormat(Properties.Resources.SEARCH_RECALL, orgUnitGUID);

            if (this.Running)
            {
                this.runningSearches--;
                this.Searches[orgUnitGUID] = true;
                this.OnRecall(this, orgUnitGUID);
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            this.runningSearches++;

            Log.DebugFormat(Properties.Resources.SEARCH_RECALL_START_SERVICE, this.RunningSearches);

            if (!this.Running)
            {
                Log.Info(Properties.Resources.LOG_START_SERVICE);

                this.Timeout = 600000;
                base.StartService();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            Log.Info(Properties.Resources.LOG_STOP_SERVICE);

            this.timeout = 0;
            this.runningSearches = 0;
            this.Cancel = cancel;

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running && this.RunningSearches > 0 && this.Timeout > 0)
            {
                Log.DebugFormat(
                    Properties.Resources.SEARCH_RECALL_THREAD_STATE,
                    this.Running,
                    this.runningSearches,
                    this.Timeout);

                this.timeout -= 1000;
                Thread.Sleep(1000);
            }

            Log.InfoFormat(
                    Properties.Resources.SEARCH_RECALL_THREAD_STATE_END,
                    this.Running,
                    this.runningSearches,
                    this.Timeout);
        }
    }
}