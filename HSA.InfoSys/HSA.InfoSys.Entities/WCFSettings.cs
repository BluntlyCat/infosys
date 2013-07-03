// ------------------------------------------------------------------------
// <copyright file="WCFSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// There we can store settings for WCF controller host.
    /// </summary>
    [DataContract]
    [Serializable]
    public class WCFSettings : Settings
    {
        /// <summary>
        /// The certificate password.
        /// </summary>
        private byte[] certificatePassword;

        /// <summary>
        /// Gets or sets the HTTP host.
        /// </summary>
        /// <value>
        /// The HTTP host.
        /// </value>
        [DataMember]
        public virtual string HttpHost { get; set; }

        /// <summary>
        /// Gets or sets the HTTP port.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        [DataMember]
        public virtual int HttpPort { get; set; }

        /// <summary>
        /// Gets or sets the HTTP path.
        /// </summary>
        /// <value>
        /// The HTTP path.
        /// </value>
        [DataMember]
        public virtual string HttpPath { get; set; }

        /// <summary>
        /// Gets or sets the net TCP host.
        /// </summary>
        /// <value>
        /// The net TCP host.
        /// </value>
        [DataMember]
        public virtual string NetTcpHost { get; set; }

        /// <summary>
        /// Gets or sets the net TCP port.
        /// </summary>
        /// <value>
        /// The net TCP port.
        /// </value>
        [DataMember]
        public virtual int NetTcpPort { get; set; }

        /// <summary>
        /// Gets or sets the net TCP path.
        /// </summary>
        /// <value>
        /// The net TCP path.
        /// </value>
        [DataMember]
        public virtual string NetTcpPath { get; set; }

        /// <summary>
        /// Gets or sets the certificate path.
        /// </summary>
        /// <value>
        /// The certificate path.
        /// </value>
        [DataMember]
        public virtual string CertificatePath { get; set; }

        /// <summary>
        /// Gets or sets the certificate password.
        /// </summary>
        /// <value>
        /// The certificate password.
        /// </value>
        [DataMember]
        public virtual byte[] CertificatePassword
        {
            get
            {
                return this.certificatePassword;
            }

            set
            {
                this.certificatePassword = value;
            }
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        /// <param name="httpHost">The HTTP host.</param>
        /// <param name="httpPort">The HTTP port.</param>
        /// <param name="httpPath">The HTTP path.</param>
        /// <param name="netTcpHost">The net TCP host.</param>
        /// <param name="netTcpPort">The net TCP port.</param>
        /// <param name="netTcpPath">The net TCP path.</param>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="password">The password.</param>
        public virtual void InitializeSettings(
            string httpHost,
            int httpPort,
            string httpPath,
            string netTcpHost,
            int netTcpPort,
            string netTcpPath,
            string certificatePath = null,
            string password = null)
        {
            this.HttpHost = httpHost;
            this.HttpPort = httpPort;
            this.HttpPath = httpPath;

            this.NetTcpHost = netTcpHost;
            this.NetTcpPort = netTcpPort;
            this.NetTcpPath = netTcpPath;

            this.CertificatePath = certificatePath;

            if (password != null)
            {
                this.CertificatePassword = Encryption.Encrypt(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.WCF_SETTINGS_TO_STRING,
                this.HttpHost,
                this.HttpPort,
                this.HttpPath,
                this.NetTcpHost,
                this.NetTcpPort,
                this.NetTcpPath,
                this.CertificatePath,
                this.SizeOf());
        }

        /// <summary>
        /// Sets the default values.
        /// </summary>
        public override void SetDefaults()
        {
            this.HttpHost = "localhost";
            this.HttpPort = 8085;
            this.HttpPath = "CrawlerProxy";

            this.NetTcpHost = "localhost";
            this.NetTcpPort = 8086;
            this.NetTcpPath = "CrawlerProxy";

            this.CertificatePath = "Certificates/InfoSys.pfx";
            this.CertificatePassword = Encryption.Encrypt(Encoding.UTF8.GetBytes("Aes2xe1baetei8Y"));
        }
    }
}
