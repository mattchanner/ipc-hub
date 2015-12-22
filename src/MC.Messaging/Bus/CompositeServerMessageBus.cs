using System;
using System.Collections.Generic;

using MC.Messaging.Common;
using MC.Messaging.Transport.Pipes;

namespace MC.Messaging.Bus
{
    /// <summary>
    /// An implementation of the <see cref="ICompositeServerMessageBus"/>
    /// </summary>
    public class CompositeServerMessageBus : ICompositeServerMessageBus
    {
        /// <summary>
        /// The transport factory.
        /// </summary>
        private readonly ITransportFactory transportFactory;

        /// <summary>
        /// A cache of buses
        /// </summary>
        private IDictionary<Guid, IServerMessageBus> busCache = new Dictionary<Guid, IServerMessageBus>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeServerMessageBus"/> class.
        /// </summary>
        public CompositeServerMessageBus() : this(new NamedPipeTransportFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeServerMessageBus"/> class.
        /// </summary>
        /// <param name="transportFactory">
        /// The transport factory.
        /// </param>
        public CompositeServerMessageBus(ITransportFactory transportFactory)
        {
            this.transportFactory = transportFactory;
        }

        /// <summary>
        /// Gets or creates a new instance of the server message bus that is able to 
        /// send and receive messages from a client.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        /// <returns> The <see cref="IServerMessageBus"/>. </returns>
        public IServerMessageBus GetOrCreate(Guid clientId)
        {
            IServerMessageBus bus;
            
            if (!this.busCache.TryGetValue(clientId, out bus))
            {
                bus = new ServerMessageBus(clientId, this.transportFactory);
                this.busCache.Add(clientId, bus);
            }

            return bus;
        }
    }
}