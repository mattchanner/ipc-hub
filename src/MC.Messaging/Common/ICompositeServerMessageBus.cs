using System;

namespace MC.Messaging.Common
{
    /// <summary>
    /// Represents a server side bus that can have multiple communicate with multiple clients
    /// </summary>
    public interface ICompositeServerMessageBus
    {
        /// <summary>
        /// Gets or creates a new instance of the server message bus that is able to send and receive messages
        /// from a client.
        /// </summary>
        /// <param name="clientId"> The client id. </param>
        /// <returns> The <see cref="IServerMessageBus"/>. </returns>
        IServerMessageBus GetOrCreate(Guid clientId);
    }
}
