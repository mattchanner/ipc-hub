using System;
using System.Collections.Generic;

using MC.Messaging.Common;

namespace MC.Messaging.Serialization.Xml
{
    /// <summary>
    /// A factory responsible for creating XML based serializer instances.
    /// </summary>
    public class XmlMessageSerializerFactory : IMessageSerializerFactory
    {
        /// <summary> The internal cache of predefined serializer instances </summary>
        private IDictionary<Type, object> serializers = new Dictionary<Type, object>();

        /// <summary>
        /// Creates a new serializer for the given type
        /// </summary>
        /// <typeparam name="T">The type to be serializer</typeparam>
        /// <returns>The message serializer</returns>
        public IMessageSerializer<T> CreateSerializer<T>() where T : class
        {
            Type type = typeof(T);

            object serializerObject;
            if (this.serializers.TryGetValue(type, out serializerObject))
            {
                serializerObject = new XmlMessageSerializer<T>();
                this.serializers.Add(type, serializerObject);
            }

            return (XmlMessageSerializer<T>)serializerObject;
        }

        /// <summary>
        /// Creates a new serializer for the given type
        /// </summary>
        /// <param name="messageType">The type of message to create the serializer for</param>
        /// <returns>The message serializer</returns>
        public IMessageSerializer CreateSerializer(Type messageType)
        {
            object serializerObject;

            if (!this.serializers.TryGetValue(messageType, out serializerObject))
            {
                Type genericInstance = typeof(XmlMessageSerializer<>).MakeGenericType(messageType);
                serializerObject = Activator.CreateInstance(genericInstance);
                this.serializers.Add(messageType, serializerObject);
            }

            return (IMessageSerializer)serializerObject;
        }
    }
}
