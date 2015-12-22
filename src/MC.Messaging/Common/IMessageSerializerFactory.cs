using System;

namespace MC.Messaging.Common
{
    /// <summary>
    /// A factory for creating message serializer instances.
    /// </summary>
    public interface IMessageSerializerFactory
    {
        /// <summary>
        /// Creates a new serializer for the given type
        /// </summary>
        /// <typeparam name="T">The type to be serializer</typeparam>
        /// <returns>The message serializer</returns>
        IMessageSerializer<T> CreateSerializer<T>() where T : class;

        /// <summary>
        /// Creates a new serializer for the given type
        /// </summary>
        /// <param name="messageType">The type of message to create the serializer for</param>
        /// <returns>The message serializer</returns>
        IMessageSerializer CreateSerializer(Type messageType);
    }
}
