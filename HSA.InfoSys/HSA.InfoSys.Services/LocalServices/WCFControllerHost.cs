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
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class provides the service host for WCF.
    /// </summary>
    public class WCFControllerHost
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("WCFControllerHost");

        /// <summary>
        /// The service host for communication between server and GUI.
        /// </summary>
        private static readonly Dictionary<Service, ServiceHost> Hosts = new Dictionary<Service, ServiceHost>();

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly WCFSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="WCFControllerHost" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public WCFControllerHost(WCFSettings settings)
        {
            this.settings = settings;

            //// Initialize WCF addresses
            WCFControllerAddresses.Initialize(this.settings);
        }

        /// <summary>
        /// Creates the bindings, certificate and the service host
        /// adds the endpoint and metadata behavior to the service host
        /// and finally opens the service host.
        /// </summary>
        /// <typeparam name="T">The implementing class.</typeparam>
        /// <typeparam name="TIT">The interface for service contract.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>The T instance what was registered at WCF service.</returns>
        public T OpenWCFHost<T, TIT>(T instance) where T : Service
        {
            if (Hosts.ContainsKey(instance))
            {
                return Hosts.Keys.Single(i => i.GetType() == instance.GetType()) as T;
            }

            try
            {
                var binding = new NetTcpBinding();

                var certificate = new X509Certificate2(this.settings.CertificatePath, Encryption.Decrypt(this.settings.CertificatePassword));

                var netTcpAddress = WCFControllerAddresses.GetNetTcpAddress(typeof(TIT));
                var httpAddress = WCFControllerAddresses.GetHttpAddress(typeof(TIT));

                var host = new ServiceHost(instance, new Uri(netTcpAddress));

                var quotas = new System.Xml.XmlDictionaryReaderQuotas
                    {
                        MaxBytesPerRead = 1024 * 1024,
                        MaxArrayLength = 4096,
                        MaxStringContentLength = 1024 * 1024
                    };

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                binding.ReaderQuotas = quotas;
                binding.MaxReceivedMessageSize = 10240000;

                host.AddServiceEndpoint(
                    typeof(TIT),
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

                Hosts.Add(instance, host);

                Log.InfoFormat(Properties.Resources.WCF_CONTROLLER_WCF_HOST_OPENED, typeof(T).Name);
            }
            catch (Exception e)
            {
                throw new OpenWCFHostException(e, this.GetType().Name, instance.GetType().Name);
            }

            return instance;
        }

        /// <summary>
        /// Closes the WCF hosts.
        /// </summary>
        public void CloseWCFHosts()
        {
            foreach (var h in Hosts.Values)
            {
                h.Close();
                Log.InfoFormat(Properties.Resources.WCF_CONTROLLER_WCF_HOST_CLOSED, h.SingletonInstance.GetType().Name);
            }
        }
    }
}
