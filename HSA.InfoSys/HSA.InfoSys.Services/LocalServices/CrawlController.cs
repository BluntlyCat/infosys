// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the GUI by using WCF.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CrawlController : Service, ICrawlController
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
        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrawlController"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private CrawlController(Guid serviceGUID) : base(serviceGUID)
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
                    crawlController = new CrawlController(Guid.NewGuid());
                }

                return crawlController;
            }
        }

        /// <summary>
        /// Registers a new service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="service">The new service.</param>
        public void RegisterService(Type type, IService service)
        {
            this.services.Add(type, service);
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        /// <param name="type">The type of the service to start.</param>
        /// <returns>
        /// True indicates that the services are started.
        /// </returns>
        public bool StartService(Type type)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_START);

            if (this.services.ContainsKey(type))
            {
                this.services[type].StartService();
                return this.services[type].Running;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        /// <param name="type">The type of the service to stop.</param>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <returns>
        /// False indicates that the services are stopped.
        /// </returns>
        public bool StopService(Type type, bool cancel = false)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);

            if (this.services.ContainsKey(type))
            {
                this.services[type].StopService(cancel);
                return this.services[type].Running;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        /// <returns>True indicates that the services are started.</returns>
        public bool StartServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_START);

            foreach (var service in this.services.Values)
            {
                service.StartService();
            }

            return true;
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        /// <returns>False indicates that the services are stopped.</returns>
        public bool StopServices(bool cancel = false)
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);

            foreach (var service in this.services.Values)
            {
                service.StopService(cancel);
            }

            return false;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }
    }
}
