using System;

namespace MC.Messaging.Common
{
   /// <summary>
    /// A class responsible for the construction of the underlying transport mechanism
    /// </summary>
    public interface ITransportFactory
    {
        /// <summary>
        /// Constructs the <see cref="IMessageTransport{T}"/> for a client
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <typeparam name="T"> The type of message to be transported </typeparam>
        /// <returns> The message transport instance for the client </returns>
        IMessageTransport<T> CreateClientTransport<T>(Guid clientId) where T : class;

        /// <summary>
        /// Constructs the <see cref="IMessageTransport{T}"/> for a server
        /// </summary>
        /// <param name="clientId"> The client Id. </param>
        /// <typeparam name="T"> The type of message to be transported </typeparam>
        /// <returns> The message transport instance for the server </returns>
        IMessageTransport<T> CreateServerTransport<T>(Guid clientId) where T : class;
    }
}
