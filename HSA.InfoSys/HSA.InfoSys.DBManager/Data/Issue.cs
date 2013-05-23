// ------------------------------------------------------------------------
// <copyright file="Issue.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The description of a security issue for a component.
    /// </summary>
    [DataContract]
    public class Issue : Entity
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [DataMember]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the threat level.
        /// </summary>
        /// <value>
        /// The threat level.
        /// </value>
        [DataMember]
        public virtual int ThreatLevel { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DataMember]
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        [DataMember]
        public virtual Component Component { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [DataMember]
        public virtual Source Source { get; set; }

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
                    if (type == typeof(Source))
                    {
                        this.Source = this.Source.Unproxy() as Source;
                    }
                    else if (type == typeof(Component))
                    {
                        this.Component = this.Component.Unproxy() as Component;
                        this.Component.Unproxy(types);
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
                "{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                this.EntityId,
                this.Text,
                this.Title,
                this.ThreatLevel,
                this.Date,
                this.Component,
                this.Source);
        }
    }
}
