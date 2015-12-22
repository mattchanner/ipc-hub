using System;

using MC.Messaging.Common;
using MC.Messaging.Transport.Pipes;

namespace MC.Messaging.Bus
{
    /// <summary>
    /// Represents a client message bus
    /// </summary>
    public class ClientMessageBus : IClientMessageBus
    {
        /// <summary> The transport factory. </summary>
        private ITransportFactory transportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMessageBus"/> class.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        public ClientMessageBus(Guid clientId) : this(clientId, new NamedPipeTransportFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMessageBus"/> class.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        /// <param name="transportFactory"> The transport factory. </param>
        public ClientMessageBus(Guid clientId, ITransportFactory transportFactory)
        {
            this.transportFactory = transportFactory;
            this.ClientId = clientId;
        }

        /// <summary>
        /// Gets the client identifier
        /// </summary>
        public Guid ClientId { get; private set; }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <typeparam name="T">The type of message to send</typeparam>
        /// <param name="message">The message to send</param>
        public void Publish<T>(T message) where T : class
        {
            this.transportFactory.CreateClientTransport<T>(this.ClientId).WriteMessage(message);
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
            IMessageTransport<TResponse> transport = 
                this.transportFactory.CreateClientTransport<TResponse>(this.ClientId);
            
            transport.MessageReceived += receiver;

            return new AutoUnregisterEvent<TResponse>(transport, receiver);
        }
    }
}
