// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.DBManager
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager.Data;
    using System.Runtime.Serialization;

    /// <summary>
    /// The interface for accessing the data base.
    /// </summary>
    [ServiceContract]
    public interface IDBManager
    {
        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        [OperationContract]
        void AddEntity(object entity);

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        [OperationContract]
        void UpdateEntity(object entity);

        [OperationContract]
        Component GetComponent(Guid componentGuid);

        [OperationContract]
        Source GetSource(Guid sourceGuid);

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        [OperationContract]
        Component CreateComponent(string name, string category);

        /// <summary>
        /// Creates an source object.
        /// </summary>
        /// <param name="sourceURL">The URL where the source points to.</param>
        /// <returns>
        /// The created source object.
        /// </returns>
        [OperationContract]
        Source CreateSource(string sourceURL);
    }
}
