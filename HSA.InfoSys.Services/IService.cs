// ------------------------------------------------------------------------
// <copyright file="IService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    /// <summary>
    /// This interface provides all methods for controlling a service.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IService"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        bool Running { get; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        void StartService();

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        void StopService(bool cancel = false);
    }
}
