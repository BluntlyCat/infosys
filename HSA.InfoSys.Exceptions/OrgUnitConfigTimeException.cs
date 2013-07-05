// ------------------------------------------------------------------------
// <copyright file="OrgUnitConfigTimeException.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Exceptions
{
    using System;

    /// <summary>
    /// Throws an exception if day or time value is zero or less.
    /// </summary>
    public sealed class OrgUnitConfigTimeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrgUnitConfigTimeException"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public OrgUnitConfigTimeException(string name) : base(Properties.Resources.ORGUNIT_CONFIG_TIME_ZERO_EXCEPTION)
        {
            this.Source = name;
        }
    }
}
