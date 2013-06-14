// ------------------------------------------------------------------------
// <copyright file="IScheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.Common.Entities;

    /// <summary>
    /// This interface provides functionality for
    /// adding or removing OrgUnitConfig objects
    /// </summary>
    [ServiceContract]
    public interface IScheduler
    {
        /// <summary>
        /// Adds the OrgUnitConfig.
        /// </summary>
        /// <param name="orgConfig">The OrgUnitConfig.</param>
        [OperationContract]
        void AddOrgUnitConfig(OrgUnitConfig orgConfig);

        /// <summary>
        /// Removes the OrgUnitConfig.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The OrgUnitConfigGUID.</param>
        [OperationContract]
        void RemoveOrgUnitConfig(Guid orgUnitConfigGUID);
    }
}
