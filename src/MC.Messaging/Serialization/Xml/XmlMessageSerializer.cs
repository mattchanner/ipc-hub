using System.IO;
using System.Xml.Serialization;

using MC.Messaging.Common;

namespace MC.Messaging.Serialization.Xml
{
    /// <summary>
    /// An XML based message serializer.
    /// </summary>
    /// <typeparam name="T">The type of message to read and write </typeparam>
    public class XmlMessageSerializer<T> : IMessageSerializer<T> where T : class
    {
        /// <summary> The serializer type </summary>
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(T));

        /// <summary>
        /// Writes a message to the writer
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <returns>The byte representation of the message</returns>
        public byte[] Write(T message)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.serializer.Serialize(memoryStream, message);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Writes a message to the writer
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <returns>The byte representation of the message</returns>
        public byte[] Write(object message)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.serializer.Serialize(memoryStream, message);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Reads a message from the underlying stream, returning the result as an object instance.
        /// </summary>
        /// <param name="message">The message to read</param>
        /// <returns>The read object</returns>
        object IMessageSerializer.Read(byte[] message)
        {
            return this.Read(message);
        }

        /// <summary>
        /// Reads a message from the underlying stream, returning the result as an object instance.
        /// </summary>
        /// <param name="message">The message to read</param>
        /// <returns>The read object</returns>
        public T Read(byte[] message)
        {
            using (MemoryStream memoryStream = new MemoryStream(message))
            {
                memoryStream.Position = 0;
                return (T)this.serializer.Deserialize(memoryStream);
            }
        }
    }
}
