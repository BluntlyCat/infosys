// ------------------------------------------------------------------------
// <copyright file="CrawlControllerHost.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class provides the service host for WCF.
    /// </summary>
    public class CrawlControllerHost
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlControllerHost");

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private List<ServiceHost> hosts = new List<ServiceHost>();

        /// <summary>
        /// Creates the bindings, certificate and the service host
        /// adds the endpoint and metadata behavior to the service host
        /// and finally opens the service host.
        /// </summary>
        /// <typeparam name="T">The implementing class.</typeparam>
        /// <typeparam name="IT">The interface for service contract.</typeparam>
        public void OpenWCFHost<T, IT>()
        {
            var binding = new NetTcpBinding();
            X509Certificate2 certificate;

#if !MONO
            certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_DOTNET, "Aes2xe1baetei8Y");
#else
            certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_MONO, "Aes2xe1baetei8Y");
#endif

            var netTcpAddress = Addresses.GetNetTcpAddress(typeof(IT));
            var httpAddress = Addresses.GetHttpAddress(typeof(IT));

            var host = new ServiceHost(typeof(T), new Uri(httpAddress));

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            host.AddServiceEndpoint(
                typeof(IT),
                binding,
                netTcpAddress);

            host.Credentials.ServiceCertificate.Certificate = certificate;

            var metadataBevavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (metadataBevavior == null)
            {
                metadataBevavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadataBevavior);
            }

            var mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
            host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                mexBinding,
                httpAddress);

            host.Open();

            this.hosts.Add(host);
            
            Log.InfoFormat(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_OPENED, typeof(T).Name);
        }

        /// <summary>
        /// Closes the WCF hosts.
        /// </summary>
        public void CloseWCFHosts()
        {
            foreach (var h in this.hosts)
            {
                h.Close();
            }

            Log.Info(Properties.Resources.CRAWL_CONTROLLER_WCF_HOST_CLOSED);
        }
    }
}
