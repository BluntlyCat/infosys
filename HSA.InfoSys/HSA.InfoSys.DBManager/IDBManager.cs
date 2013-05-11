namespace HSA.InfoSys.DBManager
{
    using System;
    using HSA.InfoSys.DBManager.Data;

    public interface IDBManager
    {
        /// <summary>
        /// Adds the new object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void AddNewObject(object obj);

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void UpdateObject(object obj);

        /// <summary>
        /// Gets the component by id.
        /// </summary>
        /// <param name="componentGUID">The component GUID.</param>
        /// <returns>The component.</returns>
        Component GetComponent(Guid componentGUID);

        T GetEntity<T>(Guid entityGuid);

        /// <summary>
        /// Gets the issue by id.
        /// </summary>
        /// <param name="issueGUID">The issue GUID.</param>
        /// <returns>The issue.</returns>
        Issue GetIssue(Guid issueGUID);

        /// <summary>
        /// Gets the source by id.
        /// </summary>
        /// <param name="sourceGUID">The source GUID.</param>
        /// <returns>The source.</returns>
        Source GetSource(Guid sourceGUID);

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <returns>The component.</returns>
        Component CreateComponent(string name, string category);

        /// <summary>
        /// Creates the source.
        /// </summary>
        /// <param name="URL">The URL.</param>
        /// <returns>The source.</returns>
        Source CreateSource(string URL);
    }
}
