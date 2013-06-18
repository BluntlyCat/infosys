// ------------------------------------------------------------------------
// <copyright file="NetDataContractOperationBehavior.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.NetDataContractSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Xml;

    /// <summary>
    /// This is the operation behavior of the NetDataContractSerializer
    /// </summary>
    public class NetDataContractOperationBehavior : DataContractSerializerOperationBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetDataContractOperationBehavior"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public NetDataContractOperationBehavior(OperationDescription operation) : base(operation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDataContractOperationBehavior"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="dataContractFormatAttribute">The data contract format attribute.</param>
        public NetDataContractOperationBehavior(
            OperationDescription operation,
            DataContractFormatAttribute dataContractFormatAttribute)
            : base(operation, dataContractFormatAttribute)
        {
        }

        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="knownTypes">The known types.</param>
        /// <returns>The XML object serializer.</returns>
        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new NetDataContractSerializer(name, ns);
        }

        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="knownTypes">The known types.</param>
        /// <returns>The XML object serializer.</returns>
        public override XmlObjectSerializer CreateSerializer(
            Type type,
            XmlDictionaryString name,
            XmlDictionaryString ns,
            IList<Type> knownTypes)
        {
            return new NetDataContractSerializer(name, ns);
        }
    }
}
