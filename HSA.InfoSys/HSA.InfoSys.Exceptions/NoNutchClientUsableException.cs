// ------------------------------------------------------------------------
// <copyright file="NoNutchClientUsableException.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Exceptions
{
    using System;

    /// <summary>
    /// Throws an exception if all clients returned
    /// an error while trying to connect over SSH.
    /// </summary>
    public sealed class NoNutchClientUsableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoNutchClientUsableException"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NoNutchClientUsableException(string name)
            : base(Properties.Resources.NO_USABLE_NUTCH_CLIENT_EXCEPTION)
        {
            this.Source = name;
        }
    }
}
