// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.SolrClient;
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
        private static readonly ILog Log = Logging.GetLogger("CrawlController");

        /// <summary>
        /// The database manager.
        /// </summary>
        private static IDBManager dbManager = DBManager.GetDBManager();

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// Our delegate for invoking an async callback.
        /// </summary>
        /// <param name="query">The query.</param>
        public delegate void InvokeSolrSearch(string query);

        /// <summary>
        /// Gets the crawl controller proxy.
        /// </summary>
        /// <returns>An ICrawlController.</returns>
        public static ClientProxy ClientProxy
        {
            get
            {
                var address = new EndpointAddress("net.tcp://192.168.0.9:8085/CrawlerProxy/");
                var binding = new NetTcpBinding(SecurityMode.Transport);
                var proxy = new ClientProxy(binding, address);

                return proxy;
                /*return ChannelFactory<ICrawlController>.CreateChannel(
                    new NetTcpBinding(SecurityMode.Transport),
                    new EndpointAddress(Properties.Settings.Default.NET_TCP_ADDRESS),
                    new Uri(Properties.Settings.Default.NET_TCP_ADDRESS));*/
            }
        }

        /// <summary>
        /// Creates the bindings, certificate and the service host
        /// adds the endpoint and metadata behavior to the service host
        /// and finally opens the service host.
        /// </summary>
        public void OpenWCFHost()
        {
            var binding = new NetTcpBinding();
            X509Certificate2 certificate;

#if !MONO
            certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_DOTNET);
#else
            certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_MONO);
#endif

            this.host = new ServiceHost(typeof(CrawlController), new Uri("http://192.168.0.9:8086/CrawlerProxy/"));

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            this.host.AddServiceEndpoint(
                typeof(ICrawlController),
                binding,
                "net.tcp://192.168.0.9:8085/CrawlerProxy/");

            this.host.Credentials.ServiceCertificate.Certificate = certificate;

            var metadataBevavior = this.host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (metadataBevavior == null)
            {
                metadataBevavior = new ServiceMetadataBehavior();
                this.host.Description.Behaviors.Add(metadataBevavior);
            }

            this.host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexHttpBinding(),
                "http://192.168.0.9:8086/CrawlerProxy/");

            this.host.Open();

            Log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_OPENED);
        }

        /// <summary>
        /// Closes the WCF host.
        /// </summary>
        public void CloseWCFHost()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_CLOSED);
            this.host.Close();
        }

        #region ICrawlController Service Contract

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <param name="query">The search query pattern.</param>
        public void StartSearch(string query)
        {
            Log.InfoFormat(Properties.Resources.CRAWL_CONTROLLER_SEARCH_STARTED, query);

            var client = new SolrClient(
                Properties.Settings.Default.SOLR_PORT,
                Properties.Settings.Default.SOLR_HOST);

            ////Here we tell our delegate which method to call.
            InvokeSolrSearch invokeSearch = new InvokeSolrSearch(client.StartSearch);

            ////This is our callback method which will be
            ////called when solr finished the searchrequest.
            AsyncCallback callback = new AsyncCallback(
                c =>
                {
                    if (c.IsCompleted)
                    {
                        Log.InfoFormat("Response for query [{0}] is\r\n[{1}]", query, client.GetResponse());
                    }
                });

            invokeSearch.BeginInvoke(query, callback, this);
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            Log.Info(Properties.Resources.CRAWL_CONTROLLER_START);
            this.host = new ServiceHost(typeof(CrawlController));
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
            Log.DebugFormat("Add new entity: [{0}]", entity);
            return dbManager.AddEntity(entity);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The GUID of the updated entity.</returns>
        public Guid UpdateEntity(Entity entity)
        {
            Log.DebugFormat("Update new entity: [{0}]", entity);
            return dbManager.UpdateEntity(entity);
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
            Log.DebugFormat("Get new entity by GUID: [{0}]", entityGuid);
            return dbManager.GetEntity<Entity>(entityGuid);
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
            Log.DebugFormat("Create new component: [{0}], [{1}]", name, category);
            return dbManager.CreateComponent(name, category);
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
            Log.DebugFormat("Create new source: [{0}]", sourceURL);
            return dbManager.CreateSource(sourceURL);
        }

        #endregion
    }
}
