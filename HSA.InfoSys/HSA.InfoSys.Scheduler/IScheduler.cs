// ------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Scheduling
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.DBManager.Data;

    /// <summary>
    /// This interface provides functionality for
    /// adding or removing scheduler time objects
    /// </summary>
    [ServiceContract]
    public interface IScheduler
    {
        /// <summary>
        /// Adds the scheduler time.
        /// </summary>
        /// <param name="schedulerTime">The scheduler time.</param>
        [OperationContract]
        void AddSchedulerTime(SchedulerTime schedulerTime);

        /// <summary>
        /// Removes the scheduler time.
        /// </summary>
        /// <param name="schedulerTimeGUID">The scheduler time GUID.</param>
        [OperationContract]
        void RemoveSchedulerTime(Guid schedulerTimeGUID);
    }
}
