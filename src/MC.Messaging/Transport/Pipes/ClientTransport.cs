using System;

using MC.Messaging.Common;

namespace MC.Messaging.Transport.Pipes
{
    /// <summary>
    /// Reports a client transport
    /// </summary>
    /// <typeparam name="T"> The type of message to transport </typeparam>
    public class ClientTransport<T> : IMessageTransport<T> where T : class
    {
        /// <summary> The pipe client. </summary>
        private readonly NamedPipeClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTransport{T}"/> class.
        /// </summary>
        /// <param name="client"> The client. </param>
        public ClientTransport(NamedPipeClient client)
        {
            this.client = client;
            client.Router.AddMessageRouter<T>(this.HandleMessageReceived);
        }

        /// <summary>
        /// The message received event
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets or sets the serializer to use for reading and writing messages to the underlying transport mechanism.
        /// </summary>
        public IMessageSerializer<T> Serializer { get; set; }

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"> The message to send. </param>
        public void WriteMessage(T message)
        {
            this.client.Write(message);
        }

        /// <summary>
        /// Handles the message received from the underlying transport method in order to raise events up to the caller.
        /// </summary>
        /// <param name="message"> The message to raise. </param>
        private void HandleMessageReceived(T message)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, new MessageReceivedEventArgs<T>(message));
            }
        }
    }
}
