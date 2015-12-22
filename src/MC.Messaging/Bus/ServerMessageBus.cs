using System;

using MC.Messaging.Common;
using MC.Messaging.Transport.Pipes;

namespace MC.Messaging.Bus
{
    /// <summary>
    /// An implementation of the server message bus
    /// </summary>
    public class ServerMessageBus : IServerMessageBus
    {
        /// <summary> The transport factory. </summary>
        private ITransportFactory transportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessageBus"/> class.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        public ServerMessageBus(Guid clientId) : this(clientId, new NamedPipeTransportFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerMessageBus"/> class.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        /// <param name="transportFactory"> The transport factory. </param>
        public ServerMessageBus(Guid clientId, ITransportFactory transportFactory)
        {
            this.transportFactory = transportFactory;
            this.ClientId = clientId;
        }

        /// <summary>
        /// Gets the client identifier
        /// </summary>
        public Guid ClientId { get; private set; }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <typeparam name="T">The type of message to send</typeparam>
        /// <param name="message">The message to send</param>
        public void Publish<T>(T message) where T : class
        {
            var transport = this.transportFactory.CreateServerTransport<T>(this.ClientId);
            transport.WriteMessage(message);
        }

        /// <summary>
        /// Registers a handler to receive messages of a given type.
        /// </summary>
        /// <typeparam name="TResponse">The response type to handle</typeparam>
        /// <param name="receiver">The response handler</param>
        /// <returns>A token that when disposed will unregister the receiver</returns>
        public IDisposable Subscribe<TResponse>(
            EventHandler<MessageReceivedEventArgs<TResponse>> receiver) where TResponse : class
        {
            IMessageTransport<TResponse> transport = this.transportFactory.CreateServerTransport<TResponse>(this.ClientId);
            
            transport.MessageReceived += receiver;

            return new AutoUnregisterEvent<TResponse>(transport, receiver);
        }
    }
}
