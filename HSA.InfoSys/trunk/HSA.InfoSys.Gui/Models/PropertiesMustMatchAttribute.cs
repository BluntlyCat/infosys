// ------------------------------------------------------------------------
// <copyright file="PropertiesMustMatchAttribute.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    /// <summary>
    /// This class compares properties if they are equal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        /// <summary>
        /// The default error message
        /// </summary>
        //// private readonly string DefaultErrorMessage = Properties.Resources.PROPERTIE_MATCH_NOT_EQUAL;

        /// <summary>
        /// The type id.
        /// </summary>
        private readonly object typeId = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesMustMatchAttribute"/> class.
        /// </summary>
        /// <param name="originalProperty">The original property.</param>
        /// <param name="confirmProperty">The confirm property.</param>
        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty) : base(Properties.Resources.PROPERTIE_MATCH_NOT_EQUAL)
        {
            this.OriginalProperty = originalProperty;
            this.ConfirmProperty = confirmProperty;
        }

        /// <summary>
        /// Gets the confirm property.
        /// </summary>
        /// <value>
        /// The confirm property.
        /// </value>
        public string ConfirmProperty { get; private set; }

        /// <summary>
        /// Gets the original property.
        /// </summary>
        /// <value>
        /// The original property.
        /// </value>
        public string OriginalProperty { get; private set; }

        /// <summary>
        /// In the implementation of a derived class a unique identifier <see cref="T:System.Attribute" />  of this is called.
        /// </summary>
        /// <returns>A <see cref="T:System.Object" />, is the unique identifier for this attribute.</returns>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }

        /// <summary>
        /// Applies a formatting on an error message on the base of that data field in which the error appeared.
        /// </summary>
        /// <param name="name">The name which should be included.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(
                CultureInfo.CurrentUICulture,
                this.ErrorMessageString,
                this.OriginalProperty,
                this.ConfirmProperty);
        }

        /// <summary>
        /// Confirms that the object is valid.
        /// </summary>
        /// <param name="value">The value of the validated object.</param>
        /// <returns>
        /// true, if valid otherwise false.
        /// </returns>
        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(
                this.OriginalProperty,
                true /* ignoreCase */).GetValue(value);

            object confirmValue = properties.Find(this.ConfirmProperty, true /* ignoreCase */).GetValue(value);

            return object.Equals(originalValue, confirmValue);
        }
    }
}