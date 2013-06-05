// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services;
    using log4net;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the GUI by using WCF.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CrawlController : ICrawlController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlController");

        /// <summary>
        /// The crawl controller.
        /// </summary>
        private static CrawlController crawlController;

        /// <summary>
        /// The services list.
        /// </summary>
        private List<IService> services = new List<IService>();

        /// <summary>
        /// Prevents a default instance of the <see cref="CrawlController"/> class from being created.
        /// </summary>
        private CrawlController()
        {
        }

        /// <summary>
        /// Gets the crawl controller.
        /// </summary>
        /// <value>
        /// The crawl controller.
        /// </value>
        public static CrawlController ControllerFactory
        {
            get
            {
                if (crawlController == null)
                {
                    crawlController = new CrawlController();
                }

                return crawlController;
            }
        }

        /// <summary>
        /// Registers a new service.
        /// </summary>
        /// <param name="service">The new service.</param>
        public void RegisterService(IService service)
        {
            this.services.Add(service);
        }

        #region ICrawlController

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_START);

            foreach (var service in this.services)
            {
                service.StartService();
            }
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public void StopServices(bool cancel = false)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);

            foreach (var service in this.services)
            {
                service.StopService(cancel);
            }
        }

        #endregion
    }
}
