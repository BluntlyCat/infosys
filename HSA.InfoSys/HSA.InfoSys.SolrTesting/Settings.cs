// ------------------------------------------------------------------------
// <copyright file="Settings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.SolrTesting.Properties
{    
    //// Diese Klasse ermöglicht die Behandlung bestimmter Ereignisse der Einstellungsklasse:
    ////  Das SettingChanging-Ereignis wird ausgelöst, bevor der Wert einer Einstellung geändert wird.
    ////  Das PropertyChanged-Ereignis wird ausgelöst, nachdem der Wert einer Einstellung geändert wurde.
    ////  Das SettingsLoaded-Ereignis wird ausgelöst, nachdem die Einstellungswerte geladen wurden.
    ////  Das SettingsSaving-Ereignis wird ausgelöst, bevor die Einstellungswerte gespeichert werden.

    /// <summary>
    /// The settings.
    /// </summary>
    internal sealed partial class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            //// Heben Sie die  Auskommentierung der unten angezeigten Zeilen auf, um Ereignishandler zum Speichern und Ändern von Einstellungen hinzuzufügen:
            ////
            //// this.SettingChanging += this.SettingChangingEventHandler;
            ////
            //// this.SettingsSaving += this.SettingsSavingEventHandler;
        }

        /// <summary>
        /// Settings the changing event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Configuration.SettingChangingEventArgs"/> instance containing the event data.</param>
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Fügen Sie hier Code zum Behandeln des SettingChangingEvent-Ereignisses hinzu.
        }

        /// <summary>
        /// Settings the saving event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Fügen Sie hier Code zum Behandeln des SettingsSaving-Ereignisses hinzu.
        }
    }
}
