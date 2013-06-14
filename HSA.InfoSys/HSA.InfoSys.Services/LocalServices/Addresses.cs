// ------------------------------------------------------------------------
// <copyright file="Addresses.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Services;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;

    /// <summary>
    /// This is a helper class to get the correct service address by its type.
    /// </summary>
    public static class Addresses
    {
        /// <summary>
        /// The net TCP addresses
        /// </summary>
        private static Dictionary<Type, string> netTcpAddresses = new Dictionary<Type, string>();

        /// <summary>
        /// The net TCP addresses
        /// </summary>
        private static Dictionary<Type, string> httpAddresses = new Dictionary<Type, string>();

        /// <summary>
        /// The HTTP port
        /// </summary>
        private static int httpPort = 8085;

        /// <summary>
        /// The net TCP port
        /// </summary>
        private static int netTcpPort = 8086;

        /// <summary>
        /// Indicates if addresses are initialized.
        /// </summary>
        private static bool notInitialized = true;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            if (notInitialized)
            {
                var types = new Type[]
                {
                    typeof(ICrawlController),
                    typeof(ISolrController),
                    typeof(IScheduler),
                    typeof(IDBManager),
                    typeof(ISearchRecall)
                };

                foreach (var t in types)
                {
                    var netTcpAddress = string.Format(Properties.Settings.Default.NET_TCP_ADDRESS, netTcpPort);
                    var httpAddress = string.Format(Properties.Settings.Default.HTTP_ADDRESS, httpPort);

                    netTcpAddresses.Add(t, netTcpAddress);
                    httpAddresses.Add(t, httpAddress);

                    netTcpPort += 2;
                    httpPort += 2;
                }

                httpPort = 8085;
                netTcpPort = 8086;

                notInitialized = false;
            }
        }

        /// <summary>
        /// Gets the net TCP address.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the net TCP address by type.</returns>
        public static string GetNetTcpAddress(Type type)
        {
            return netTcpAddresses.ContainsKey(type) ? netTcpAddresses[type] : string.Empty;
        }

        /// <summary>
        /// Gets the HTTP address.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the http address by type.</returns>
        public static string GetHttpAddress(Type type)
        {
            return httpAddresses.ContainsKey(type) ? httpAddresses[type] : string.Empty;
        }
    }
}
