// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the gui by using wcf.
    /// </summary>
    public class CrawlController : ICrawlController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlController");

        /// <summary>
        /// The services list.
        /// </summary>
        private List<IService> services = new List<IService>();

        /// <summary>
        /// Registers a new service.
        /// </summary>
        /// <param name="service">The new service.</param>
        public void RegisterService(IService service)
        {
            this.services.Add(service);
        }

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
        public void StopServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);

            foreach (var service in this.services)
            {
                service.StopService();
            }
        }
    }
}
