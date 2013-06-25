// ------------------------------------------------------------------------
// <copyright file="WCFControllerHostSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    /// There we can store settings for WCF controller host.
    /// </summary>
    [DataContract]
    public class WCFControllerHostSettings : Entity
    {
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
        public virtual string CertificatePassword { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.WCFCONTROLLERHOSTSETTINGS_TO_STRING,
                this.CertificatePath,
                this.SizeOf());
        }
    }
}
