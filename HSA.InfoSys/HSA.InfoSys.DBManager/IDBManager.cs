// ------------------------------------------------------------------------
// <copyright file="IDBManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.DBManager
{
    using System;
    using HSA.InfoSys.DBManager.Data;

    /// <summary>
    /// The interface for accessing the data base.
    /// </summary>
    public interface IDBManager
    {
        /// <summary>
        /// Adds a new Object (Component, Issue, Source)
        /// and saves it in database.
        /// </summary>
        /// <param name="entity">The entity to add in database.</param>
        void AddNewObject(object entity);

        /// <summary>
        /// Saves changings of a object in database.
        /// </summary>
        /// <param name="entity">The entity that should be updated.</param>
        void UpdateObject(object entity);

        /// <summary>
        /// Returns a component object.
        /// </summary>
        /// <typeparam name="T">Can be any entity found in DBManager.Data.</typeparam>
        /// <param name="entityGuid">The entity GUID.</param>
        /// <returns>
        /// The component object by its GUID.
        /// </returns>
        T GetEntity<T>(Guid entityGuid);

        /// <summary>
        /// Returns a component object.
        /// </summary>
        /// <param name="componentGUID">Id of the Object</param>
        /// <returns>
        /// The component object by its GUID.
        /// </returns>
        Component GetComponent(Guid componentGUID);

        /// <summary>
        /// Return a issue object from database
        /// </summary>
        /// <param name="issueGUID">Id of the object</param>
        /// <returns>
        /// The issue object by its GUID.
        /// </returns>
        Issue GetIssue(Guid issueGUID);

        /// <summary>
        /// Returns a source object from database
        /// </summary>
        /// <param name="sourceGUID">Id of the object.</param>
        /// <returns>
        /// The source object by its GUID.
        /// </returns>
        Source GetSource(Guid sourceGUID);

        /// <summary>
        /// Creates a component object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>
        /// The created component object.
        /// </returns>
        Component CreateComponent(string name, string category);

        /// <summary>
        /// Creates an source object.
        /// </summary>
        /// <param name="sourceURL">The URL where the source points to.</param>
        /// <returns>
        /// The created source object.
        /// </returns>
        Source CreateSource(string sourceURL);
    }
}
