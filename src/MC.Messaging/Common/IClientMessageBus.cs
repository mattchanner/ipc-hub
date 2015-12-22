using System;

namespace MC.Messaging.Common
{
    /// <summary>
    /// A client side message bus
    /// </summary>
    public interface IClientMessageBus
    {
        /// <summary>
        /// Gets the client identifier
        /// </summary>
        Guid ClientId { get; }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <typeparam name="T">The type of message to send</typeparam>
        /// <param name="message">The message to send</param>
        void Publish<T>(T message) where T : class;

        /// <summary>
        /// Registers a handler to receive messages of a given type.
        /// </summary>
        /// <typeparam name="TResponse">The response type to handle</typeparam>
        /// <param name="receiver">The response handler</param>
        /// <returns>A token that when disposed will unregister the receiver</returns>
        IDisposable Subscribe<TResponse>(EventHandler<MessageReceivedEventArgs<TResponse>> receiver) 
            where TResponse : class;
    }
}
