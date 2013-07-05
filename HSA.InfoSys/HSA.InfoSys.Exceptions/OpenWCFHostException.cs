// ------------------------------------------------------------------------
// <copyright file="OpenWCFHostException.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Exceptions
{
    using System;

    /// <summary>
    /// Throws an exception while opening the WCF service host.
    /// </summary>
    public sealed class OpenWCFHostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenWCFHostException" /> class.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="name">The name.</param>
        /// <param name="service">The service.</param>
        public OpenWCFHostException(Exception e, string name, string service)
            : base(Properties.Resources.OPEN_WCF_SERVICE_EXCEPTION, e)
        {
            this.Source = name;
            this.Service = service;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        private string Service { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.OPEN_WCF_SERVICE_EXCEPTION_TO_STRING,
                this.Service,
                this.Source,
                this.InnerException);
        }
    }
}
