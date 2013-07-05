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
        /// Is needed if the certificate for WCF needs one.
        /// </summary>
        private byte[] certificatePassword;

        /// <summary>
        /// Gets or sets the HTTP host.
        /// This is the host address where we can find the wcf service.
        /// </summary>
        /// <value>
        /// The HTTP host.
        /// </value>
        [DataMember]
        public virtual string HttpHost { get; set; }

        /// <summary>
        /// Gets or sets the HTTP port.
        /// The port on which WCF is listening for creating
        /// a new proxy.
        /// </summary>
        /// <value>
        /// The HTTP port.
        /// </value>
        [DataMember]
        public virtual int HttpPort { get; set; }

        /// <summary>
        /// Gets or sets the HTTP path.
        /// The path of the WCF service.
        /// </summary>
        /// <value>
        /// The HTTP path.
        /// </value>
        [DataMember]
        public virtual string HttpPath { get; set; }

        /// <summary>
        /// Gets or sets the net TCP host.
        /// This is the host address of our WCF service
        /// for sending data between host and client.
        /// </summary>
        /// <value>
        /// The net TCP host.
        /// </value>
        [DataMember]
        public virtual string NetTcpHost { get; set; }

        /// <summary>
        /// Gets or sets the net TCP port.
        /// The port on the net TCP host is listening.
        /// </summary>
        /// <value>
        /// The net TCP port.
        /// </value>
        [DataMember]
        public virtual int NetTcpPort { get; set; }

        /// <summary>
        /// Gets or sets the net TCP path.
        /// The path to the net TCP host.
        /// </summary>
        /// <value>
        /// The net TCP path.
        /// </value>
        [DataMember]
        public virtual string NetTcpPath { get; set; }

        /// <summary>
        /// Gets or sets the certificate path.
        /// The place where the certificate is stored.
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
        /// This method is needed if the wcf service host
        /// is not listening to our default values.
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
        /// Sets the default values of this
        /// setting or if we instanciate a new
        /// instance of a setting object because
        /// NHibernate needs virtual members but
        /// it is better to do not call a virtual
        /// member in the constructor if there are
        /// other derived classes which inherits from
        /// this setting. To avoid this problem we
        /// simply call this method after instanciating.
        /// </summary>
        public virtual void SetDefaults()
        {
            var newSettings = this.GetDefaults() as WCFSettings;

            if (newSettings != null)
            {
                this.HttpHost = newSettings.HttpHost;
                this.HttpPort = newSettings.HttpPort;
                this.HttpPath = newSettings.HttpPath;

                this.NetTcpHost = newSettings.NetTcpHost;
                this.NetTcpPort = newSettings.NetTcpPort;
                this.NetTcpPath = newSettings.NetTcpPath;

                this.CertificatePath = newSettings.CertificatePath;
                this.CertificatePassword = newSettings.CertificatePassword;
            }
        }

        /// <summary>
        /// Gets the settings with default values.
        /// </summary>
        /// <returns>
        /// A new settings object with its default values.
        /// </returns>
        public virtual Settings GetDefaults()
        {
            var newSettings = new WCFSettings
                {
                    HttpHost = "localhost",
                    HttpPort = 8085,
                    HttpPath = "CrawlerProxy",
                    NetTcpHost = "localhost",
                    NetTcpPort = 8086,
                    NetTcpPath = "CrawlerProxy",
                    CertificatePath = "Certificates/InfoSys.pfx",
                    CertificatePassword = Encryption.Encrypt(Encoding.UTF8.GetBytes("Aes2xe1baetei8Y"))
                };

            return newSettings;
        }

        /// <summary>
        /// Determines whether this settings has default values.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this settings has default values; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsDefault()
        {
            return this.Equals(this.GetDefaults());
        }
    }
}
