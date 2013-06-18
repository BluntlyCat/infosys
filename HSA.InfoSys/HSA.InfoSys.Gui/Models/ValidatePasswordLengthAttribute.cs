// ------------------------------------------------------------------------
// <copyright file="ValidatePasswordLengthAttribute.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Security;

    /// <summary>
    /// This class provides functionality for password validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// The default error message
        /// </summary>
        //// private readonly string DefaultErrorMessage = Properties.Resources.VALIDATE_PASSWORD_LENGTH;

        /// <summary>
        /// The minimum characters
        /// </summary>
        private readonly int minCharacters = Membership.Provider.MinRequiredPasswordLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatePasswordLengthAttribute"/> class.
        /// </summary>
        public ValidatePasswordLengthAttribute() : base(Properties.Resources.VALIDATE_PASSWORD_LENGTH)
        {
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
                name,
                this.minCharacters);
        }

        /// <summary>
        /// Determines if the value is valid.
        /// </summary>
        /// <param name="value">The value of the validated object.</param>
        /// <returns>
        /// true, if value is valid, otherwise false.
        /// </returns>
        public override bool IsValid(object value)
        {
            string valueAsstring = value as string;
            return valueAsstring != null && valueAsstring.Length >= this.minCharacters;
        }
    }
}