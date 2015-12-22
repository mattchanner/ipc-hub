using System;
using System.IO;

using MC.Messaging.Common;
using MC.Messaging.Serialization.Xml;

namespace MC.Messaging.Transport
{
    /// <summary>
    /// A class responsible for the underlying transport message sent across the wire.
    /// </summary>
    public class MessageDispatcher
    {
        /// <summary> The factory. </summary>
        private readonly IMessageSerializerFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        public MessageDispatcher() : this(new XmlMessageSerializerFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        public MessageDispatcher(IMessageSerializerFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Writes a message to the underlying stream
        /// </summary>
        /// <param name="message"> The message to write. </param>
        /// <param name="outputStream"> The output stream to write to </param>
        public void WriteMessage(object message, Stream outputStream)
        {
            Type messageType = message.GetType();
            string typeString = messageType.AssemblyQualifiedName;

            if (typeString == null)
            {
                throw new ArgumentException(string.Format("Message of type {0} does not have a qualified name", messageType));
            }

            IMessageSerializer serializer = this.factory.CreateSerializer(messageType);
            byte[] messageBytes = serializer.Write(message);
            byte[] typeBytes = System.Text.Encoding.UTF8.GetBytes(typeString);

            // All bytes should be written in a single go to the underlying transport mechanism.  This is particularly
            // important for named pipes when in message mode as each write is treated as a separate message
            MemoryStream tempBuffer = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(tempBuffer);

            binaryWriter.Write(typeBytes.Length);
            binaryWriter.Write(messageBytes.Length);
            binaryWriter.Write(typeBytes);
            binaryWriter.Write(messageBytes);

            byte[] bytes = tempBuffer.ToArray();
            outputStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Reads a message into a typed object from the underlying stream
        /// </summary>
        /// <param name="inputStream"> The input stream to read from </param>
        /// <returns> The <see cref="object"/>. </returns>
        public object ReadMessage(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            // FIXME: This looks pretty wrong.
            BinaryReader reader = new BinaryReader(inputStream);
            
            int typeNameSize = reader.ReadInt32();
            int messageSize = reader.ReadInt32();

            byte[] typeNameBytes = reader.ReadBytes(typeNameSize);
            byte[] messageBytes = reader.ReadBytes(messageSize);

            string typeNameString = System.Text.Encoding.UTF8.GetString(typeNameBytes);

            // This will throw if the type is not known to the process
            Type messageType = Type.GetType(typeNameString);

            IMessageSerializer serializer = this.factory.CreateSerializer(messageType);
            object result = serializer.Read(messageBytes);
            return result;
        }
    }
}
