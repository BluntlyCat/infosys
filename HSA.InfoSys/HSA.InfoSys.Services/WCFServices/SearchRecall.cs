// ------------------------------------------------------------------------
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

    /// <summary>
    /// This class implements the method for
    /// indicating that a search request for 
    /// an org unit is finished.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SearchRecall : Service, ISearchRecall
    {
        /// <summary>
        /// The search recall
        /// </summary>
        private static SearchRecall searchRecall;

        /// <summary>
        /// The finished searches
        /// </summary>
        private List<Guid> finisedSearches = new List<Guid>();

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
        }

        /// <summary>
        /// The recall handler indicates that all search requests are finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void RecallHandler(object sender);

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
        /// Recalls the GUI when search for an org unit is finished.
        /// </summary>
        /// <param name="orgUnitGuid">The org unit GUID.</param>
        public void Recall(Guid orgUnitGuid)
        {
            this.ServiceMutex.WaitOne();

            if (this.Running)
            {
                this.runningSearches--;
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            this.runningSearches++;

            if (this.ServiceThread == null || !this.Running)
            {
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
            this.Running = false;
            this.timeout = 0;
            this.runningSearches = 0;

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running && this.RunningSearches > 0 && this.Timeout > 0)
            {
                this.Timeout -= 1000;
                Thread.Sleep(1000);
            }

            this.OnRecall(this);
        }

        /// <summary>
        /// Adds the finished search.
        /// </summary>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        private void AddFinishedSearch(Guid orgUnitGUID)
        {
            this.ServiceMutex.WaitOne();
            this.finisedSearches.Add(orgUnitGUID);
            this.ServiceMutex.ReleaseMutex();
        }
    }
}
