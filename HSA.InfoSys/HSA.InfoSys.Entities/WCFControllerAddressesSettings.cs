// ------------------------------------------------------------------------
// <copyright file="WCFControllerAddressesSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// There we can store settings for WCF controller addresses.
    /// </summary>
    [DataContract]
    [Serializable]
    public class WCFControllerAddressesSettings : Entity
    {
        /// <summary>
        /// Gets or sets the HTTP address.
        /// </summary>
        /// <value>
        /// The HTTP address.
        /// </value>
        [DataMember]
        public virtual string HttpAddress { get; set; }

        /// <summary>
        /// Gets or sets the net TCP address.
        /// </summary>
        /// <value>
        /// The net TCP address.
        /// </value>
        [DataMember]
        public virtual string NetTcpAddress { get; set; }

        /// <summary>
        /// Gets or sets the HTTP port.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        [DataMember]
        public virtual int HttpPort { get; set; }

        /// <summary>
        /// Gets or sets the net TCP port.
        /// </summary>
        /// <value>
        /// The net TCP port.
        /// </value>
        [DataMember]
        public virtual int NetTcpPort { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.WCFADDRESSES_TO_STRING,
                this.HttpAddress,
                this.HttpPort,
                this.NetTcpAddress,
                this.NetTcpPort,
                this.SizeOf());
        }
    }
}
