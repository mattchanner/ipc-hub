using System;

namespace MC.Messaging.Common
{
    /// <summary>
    /// Implementations of this type are responsible for transporting a typed message to an endpoint.
    /// </summary>
    /// <typeparam name="T">The type of message being transported </typeparam>
    public interface IMessageTransport<T> where T : class
    {
        /// <summary>
        /// An event that is fired whenever a message has been received
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// This method is responsible for writing a message to the underlying transport mechanism
        /// </summary>
        /// <param name="message"> The message to write. </param>
        void WriteMessage(T message);
    }
}
