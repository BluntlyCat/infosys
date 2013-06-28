// ------------------------------------------------------------------------
// <copyright file="WCFControllerClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class is for clients which want use the WCF service
    /// </summary>
    /// <typeparam name="T">The type of what the proxy must be.</typeparam>
    public class WCFControllerClient<T>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlControllerClient");

        /// <summary>
        /// Gets the client proxy.
        /// </summary>
        /// <value>
        /// The client proxy.
        /// </value>
        public static T ClientProxy
        {
            get
            {
                Log.Info("Try get new client proxy.");

                WCFControllerAddresses.Initialize();

                var netTcpAddress = new Uri(WCFControllerAddresses.GetNetTcpAddress(typeof(T)));

                var quotas = new System.Xml.XmlDictionaryReaderQuotas();
                quotas.MaxBytesPerRead = 1024 * 1024;
                quotas.MaxArrayLength = 4096;
                quotas.MaxStringContentLength = 1024 * 1024;

                var binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                binding.ReaderQuotas = quotas;
                binding.MaxReceivedMessageSize = 10240000;
                Log.Info("Create binding for proxy.");

                var address = new EndpointAddress(
                    netTcpAddress,
                    EndpointIdentity.CreateDnsIdentity("InfoSys"));

                Log.Info("Create endpoint for proxy.");

                return ChannelFactory<T>.CreateChannel(binding, address, netTcpAddress);
            }
        }
    }
}
