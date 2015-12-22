namespace MC.Messaging.Common
{
    /// <summary>
    /// Represents a serializer responsible for reading and writing messages
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Writes a message to the writer
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <returns>The byte representation of the message</returns>
        byte[] Write(object message);

        /// <summary>
        /// Reads a message from the underlying stream, returning the result as an object instance.
        /// </summary>
        /// <param name="message">The message to read</param>
        /// <returns>The read object</returns>
        object Read(byte[] message);
    }

    /// <summary>
    /// Represents a serializer responsible for reading and writing messages
    /// </summary>
    /// <typeparam name="TMessage">The type of message to read and write </typeparam>
    public interface IMessageSerializer<TMessage> : IMessageSerializer where TMessage : class
    {
        /// <summary>
        /// Writes a message to the writer
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <returns>The byte representation of the message</returns>
        byte[] Write(TMessage message);

        /// <summary>
        /// Reads a message from the underlying stream, returning the result as an object instance.
        /// </summary>
        /// <param name="message">The message to read</param>
        /// <returns>The read object</returns>
        new TMessage Read(byte[] message);
    }
}
