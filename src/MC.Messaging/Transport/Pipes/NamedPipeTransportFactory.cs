using System;
using System.Collections.Generic;

using MC.Messaging.Common;
using MC.Messaging.Serialization.Xml;

namespace MC.Messaging.Transport.Pipes
{
    /// <summary>
    /// A factory for creating instances of client and server transports using named pipes
    /// </summary>
    public class NamedPipeTransportFactory : ITransportFactory
    {
        /// <summary> The single dispatcher </summary>
        private MessageDispatcher dispatcher;

        /// <summary> The client pipe </summary>
        private IDictionary<Guid, NamedPipeClient> clientPipes = new Dictionary<Guid, NamedPipeClient>();

        /// <summary> The client pipe </summary>
        private IDictionary<Guid, NamedPipeServer> serverPipes = new Dictionary<Guid, NamedPipeServer>();

        /// <summary> A cache of the transports created for a client and message type </summary>
        private IDictionary<Guid, IDictionary<Type, object>> clientTransports = new Dictionary<Guid, IDictionary<Type, object>>();

        /// <summary> A cache of the transports created for a server and message type </summary>
        private IDictionary<Guid, IDictionary<Type, object>> serverTransports = new Dictionary<Guid, IDictionary<Type, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeTransportFactory"/> class.
        /// </summary>
        public NamedPipeTransportFactory() : this(new XmlMessageSerializerFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeTransportFactory"/> class.
        /// </summary>
        /// <param name="serializerFactory"> The serializer factory. </param>
        public NamedPipeTransportFactory(IMessageSerializerFactory serializerFactory)
        {
            this.dispatcher = new MessageDispatcher(serializerFactory);
        }

        /// <summary>
        /// Constructs the <see cref="IMessageTransport{T}"/> for a client
        /// </summary>
        /// <typeparam name="T"> The type of message to be transported </typeparam>
        /// <param name="clientId"> The client Id. </param>
        /// <returns> The message transport instance for the client </returns>
        public IMessageTransport<T> CreateClientTransport<T>(Guid clientId) where T : class
        {
            NamedPipeClient clientPipe;
            if (!this.clientPipes.TryGetValue(clientId, out clientPipe))
            {
                clientPipe = new NamedPipeClient(clientId.ToString(), this.dispatcher);
                this.clientPipes.Add(clientId, clientPipe);
                clientPipe.Start();
            }

            IDictionary<Type, object> cachedTransports;
            if (!this.clientTransports.TryGetValue(clientId, out cachedTransports))
            {
                cachedTransports = new Dictionary<Type, object>();
                this.clientTransports[clientId] = cachedTransports;
            }

            object cachedTransport;
            if (!cachedTransports.TryGetValue(typeof(T), out cachedTransport))
            {
                cachedTransport = new ClientTransport<T>(clientPipe);
                cachedTransports[typeof(T)] = cachedTransport;
            }

            return (ClientTransport<T>)cachedTransport;
        }

        /// <summary>
        /// Constructs the <see cref="IMessageTransport{T}"/> for a server
        /// </summary>
        /// <typeparam name="T">The type of message to be transported</typeparam>
        /// <param name="clientId"> The client Id. </param>
        /// <returns>The message transport instance for the server</returns>
        public IMessageTransport<T> CreateServerTransport<T>(Guid clientId) where T : class
        {
            NamedPipeServer serverPipe;
            if (!this.serverPipes.TryGetValue(clientId, out serverPipe))
            {
                serverPipe = new NamedPipeServer(clientId.ToString(), this.dispatcher);
                this.serverPipes.Add(clientId, serverPipe);
                serverPipe.Start();
            }

            IDictionary<Type, object> cachedTransports;
            if (!this.serverTransports.TryGetValue(clientId, out cachedTransports))
            {
                cachedTransports = new Dictionary<Type, object>();
                this.serverTransports[clientId] = cachedTransports;
            }

            object cachedTransport;
            if (!cachedTransports.TryGetValue(typeof(T), out cachedTransport))
            {
                cachedTransport = new ServerTransport<T>(serverPipe);
                cachedTransports[typeof(T)] = cachedTransport;
            }

            return (ServerTransport<T>)cachedTransport;
        }
    }
}
