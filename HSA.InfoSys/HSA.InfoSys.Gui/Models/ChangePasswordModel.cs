// ------------------------------------------------------------------------
// <copyright file="ChangePasswordModel.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This class provides the functionality for changing the user password.
    /// </summary>
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "Das neue Kennwort entspricht nicht dem Bestätigungskennwort.")]
    public class ChangePasswordModel
    {
        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        /// <value>
        /// The old password.
        /// </value>
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Aktuelles Kennwort")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        /// <value>
        /// The new password.
        /// </value>
        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Neues Kennwort")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        /// <value>
        /// The confirm password.
        /// </value>
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Neues Kennwort bestätigen")]
        public string ConfirmPassword { get; set; }
    }
}