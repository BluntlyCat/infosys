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
        /// Initializes a new instance of the <see cref="ClientProxy"/> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        public ClientProxy(Binding binding, EndpointAddress address) : base(binding, address)
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
    }
}
