// ------------------------------------------------------------------------
// <copyright file="Component.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;
    using NHibernate.Mapping;

    /// <summary>
    /// A component represents a system as a whole
    /// for example a web server or a database server.
    /// </summary>
    [DataContract]
    public class Component : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [DataMember]
        public virtual Result Result { get; set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// </summary>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity behind the NHibernate proxy.
        /// </returns>
        public override Entity Unproxy(Type[] types = null)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    if (type == typeof(Result))
                    {
                        this.Result = this.Result.Unproxy() as Result;
                    }
                }
            }

            return base.Unproxy(types);
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
                "{0}, {1}, {2}",
                this.EntityId,
                this.Name,
                this.Result);
        }
    }
}
