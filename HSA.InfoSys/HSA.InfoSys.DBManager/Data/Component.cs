namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using PetaPoco;

    [TableName("Component"), PrimaryKey("componentGuid", autoIncrement=false, sequenceName="string")]
    public class Component
    {
        /// <summary>
        /// Gets or sets the component GUID.
        /// </summary>
        /// <value>
        /// The component GUID.
        /// </value>
        public virtual Guid componentGUID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string name { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public virtual string category { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", componentGUID, name, category);
        }
    }
}
