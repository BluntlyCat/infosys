// ------------------------------------------------------------------------
// <copyright file="UseNetDataContractSerializer.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.NetDataContractSerializer
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// This class is the Attribute to tell WCF to use the NetDataContractSerializer.
    /// </summary>
    public class UseNetDataContractSerializer : Attribute, IOperationBehavior
    {
        /// <summary>
        /// Adds the binding parameters.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="parameters">The parameters.</param>
        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        /// <summary>
        /// Applies the client behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="proxy">The proxy.</param>
        public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            ReplaceDataContractSerializerOperationBehavior(description);
        }

        /// <summary>
        /// Applies the dispatch behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">The dispatch.</param>
        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            ReplaceDataContractSerializerOperationBehavior(description);
        }

        /// <summary>
        /// Validates the specified description.
        /// </summary>
        /// <param name="description">The description.</param>
        public void Validate(OperationDescription description)
        {
        }

        /// <summary>
        /// Replaces the data contract serializer operation behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
        {
            var dcsOperationBehavior =
            description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcsOperationBehavior != null)
            {
                description.Behaviors.Remove(dcsOperationBehavior);
                description.Behaviors.Add(new NetDataContractOperationBehavior(description));
            }
        }
    }
}
