// ------------------------------------------------------------------------
// <copyright file="ClientProxy.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    public class ClientProxy : ClientBase<ICrawlController>, ICrawlController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("ClientProxy");

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDBManager dbManager = DBManager.GetDBManager();

        public ClientProxy(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        public void StartSearch(string query)
        {
            Channel.StartSearch(query);
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            Channel.StartServices();
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_START);
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public void StopServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_SHUTDOWN);
        }

        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The GUID of the new entity.</returns>
        public Guid AddEntity(Entity entity)
        {
            return Channel.AddEntity(entity);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            return Channel.UpdateEntity(entity);
        }

        /// <summary>
        /// Gets an entity from database.
        /// </summary>
        /// <param name="entityGuid">The GUID of the entity we want from database.</param>
        /// <returns>
        /// The entity you asked for.
        /// </returns>
        public Entity GetEntity(Guid entityGuid)
        {
            return Channel.GetEntity(entityGuid);
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The new component.
        /// </returns>
        public Component CreateComponent(string name, string category)
        {
            return Channel.CreateComponent(name, category);
        }

        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="sourceURL">The source URL.</param>
        /// <returns>
        /// The new source.
        /// </returns>
        public Source CreateSource(string sourceURL)
        {
            return Channel.CreateSource(sourceURL);
        }

        /// <summary>
        /// Creates a result object
        /// </summary>
        /// <param name="data">The content of the result</param>
        /// <returns>
        /// The created result object
        /// </returns>
        public Result CreateResult(string data)
        {
            Log.DebugFormat("Create new result: [{0}]", data);
            return Channel.CreateResult(data);
        }

        /// <summary>
        /// Creates a SystemService object
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="component">A component object</param>
        /// <param name="sysconfig">A system config object</param>
        /// <returns>
        /// The created SystemService object
        /// </returns>
        public SystemService CreateSystemService(int userId, Component component, SystemConfig sysconfig)
        {
            Log.DebugFormat("Create new system service: [{0}, {1}, {2}]", userId, component, sysconfig);
            return Channel.CreateSystemService(userId, component, sysconfig);
        }

        /// <summary>
        /// Creates a SystemConfig object
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="email">The email text.</param>
        /// <param name="urlActive">if set to <c>true</c> [URL active].</param>
        /// <param name="emailNotification">if set to <c>true</c> [email notification].</param>
        /// <param name="schedulerActive">if set to <c>true</c> [scheduler active].</param>
        /// <param name="scheduler">A scheduler object.</param>
        /// <returns>
        /// The created SystemConfig object.
        /// </returns>
        public SystemConfig CreateSystemConfig(
            string url,
            string email,
            bool urlActive,
            bool emailNotification,
            bool schedulerActive,
            Scheduler scheduler)
        {
            Log.DebugFormat(
                "Create new system service: [{0}, {1}, {2}, {3}, {4}, {5}]",
                url,
                email,
                urlActive,
                emailNotification,
                schedulerActive,
                scheduler);

            return Channel.CreateSystemConfig(url, email, urlActive, emailNotification, schedulerActive, scheduler);
        }

        /// <summary>
        /// Creates a Scheduler object
        /// </summary>
        /// <param name="days">The days.</param>
        /// <param name="hours">The hours.</param>
        /// <returns>
        /// The created Scheduler object.
        /// </returns>
        public Scheduler CreateScheduler(int days, int hours)
        {
            Log.DebugFormat("Create new scheduler: [{0}, {1}]", days, hours);
            return Channel.CreateScheduler(days, hours);
        }
    }
}