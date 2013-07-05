// ------------------------------------------------------------------------
// <copyright file="WCFControllerClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class is for clients which want use the WCF service
    /// </summary>
    /// <typeparam name="T">The type of what the proxy must be.</typeparam>
    public static class WCFControllerClient<T>
    {
        /// <summary>
        /// The logger for WCFControllerClient.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("WCFControllerClient");

        /// <summary>
        /// Gets the client proxy by the type.
        /// Initialize the addresses on each call
        /// so we do not need a static configuration in xml
        /// for our clients.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The proxy of type T.</returns>
        public static T GetClientProxy(WCFSettings settings)
        {
            Log.Info(Properties.Resources.WCF_CONTROLLER_CLIENT_TRY_GET_PROXY);

            WCFControllerAddresses.Initialize(settings);

            var netTcpAddress = new Uri(WCFControllerAddresses.GetNetTcpAddress(typeof(T)));

            var quotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxBytesPerRead = 1024 * 1024,
                    MaxArrayLength = 4096,
                    MaxStringContentLength = 1024 * 1024
        };

            var binding = new NetTcpBinding
                {
                    Security =
                        {
                            Mode = SecurityMode.Transport,
                            Transport =
                                {
                                    ClientCredentialType = TcpClientCredentialType.None
                                }
                        },

                    ReaderQuotas = quotas,
                    MaxReceivedMessageSize = 10240000
                };

            Log.Info(Properties.Resources.WCF_CONTROLLER_CLIENT_CREATE_BINDING);

            var address = new EndpointAddress(
                netTcpAddress,
                EndpointIdentity.CreateDnsIdentity("InfoSys"));

            Log.Info(Properties.Resources.WCF_CONTROLLER_CLIENT_CREATE_ENDPOINT);

            return ChannelFactory<T>.CreateChannel(binding, address, netTcpAddress);
        }
    }
}
