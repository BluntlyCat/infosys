// ------------------------------------------------------------------------
// <copyright file="CrawlControllerClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class is for clients which want use the WCF service
    /// </summary>
    /// <typeparam name="T">The type of what the proxy must be.</typeparam>
    public class CrawlControllerClient<T>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("CrawlControllerClient");

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

                Addresses.Initialize();
                var netTcpAddress = new Uri(Addresses.GetNetTcpAddress(typeof(T)));

                var binding = new NetTcpBinding();
                binding.MaxReceivedMessageSize = 10485760L;
                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
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
