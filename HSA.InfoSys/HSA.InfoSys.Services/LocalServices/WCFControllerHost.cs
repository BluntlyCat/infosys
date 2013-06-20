// ------------------------------------------------------------------------
// <copyright file="WCFControllerHost.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    /// <summary>
    /// This class provides the service host for WCF.
    /// </summary>
    public class WCFControllerHost
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlControllerHost");

        /// <summary>
        /// The service host for communication between server and GUI.
        /// </summary>
        private static Dictionary<Service, ServiceHost> hosts = new Dictionary<Service, ServiceHost>();

        /// <summary>
        /// Creates the bindings, certificate and the service host
        /// adds the endpoint and metadata behavior to the service host
        /// and finally opens the service host.
        /// </summary>
        /// <typeparam name="T">The implementing class.</typeparam>
        /// <typeparam name="IT">The interface for service contract.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>The T instance what was registered at WCF service.</returns>
        public T OpenWCFHost<T, IT>(T instance) where T : Service
        {
            if (hosts.ContainsKey(instance))
            {
                return hosts.Keys.Single(i => i.GetType() == instance.GetType()) as T;
            }
            else
            {
                try
                {
                    var binding = new NetTcpBinding();
                    X509Certificate2 certificate;

                    var dir = System.Environment.CurrentDirectory;

#if !MONO
                    certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_DOTNET, "Aes2xe1baetei8Y");
#else
                    certificate = new X509Certificate2(Properties.Settings.Default.CERTIFICATE_PATH_MONO, "Aes2xe1baetei8Y");
#endif

                    var netTcpAddress = Addresses.GetNetTcpAddress(typeof(IT));
                    var httpAddress = Addresses.GetHttpAddress(typeof(IT));

                    var host = new ServiceHost(instance, new Uri(netTcpAddress));

                    var quotas = new System.Xml.XmlDictionaryReaderQuotas();
                    quotas.MaxBytesPerRead = 1024 * 1024;
                    quotas.MaxArrayLength = 4096;
                    quotas.MaxStringContentLength = 1024 * 1024;

                    binding.Security.Mode = SecurityMode.Transport;
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                    binding.ReaderQuotas = quotas;
                    binding.MaxReceivedMessageSize = 1024 * 1024;

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

                    hosts.Add(instance, host);

                    Log.InfoFormat(Properties.Resources.WCF_CONTROLLER_WCF_HOST_OPENED, typeof(T).Name);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Common error: {0}", e);
                }

                return instance;
            }
        }

        /// <summary>
        /// Closes the WCF hosts.
        /// </summary>
        public void CloseWCFHosts()
        {
            foreach (var h in hosts.Values)
            {
                h.Close();
            }

            Log.Info(Properties.Resources.WCF_CONTROLLER_WCF_HOST_CLOSED);
        }
    }
}
