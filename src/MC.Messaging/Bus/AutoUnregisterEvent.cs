using System;

using MC.Messaging.Common;

namespace MC.Messaging.Bus
{
    /// <summary>
    /// The auto unregister event.
    /// </summary>
    /// <typeparam name="TResponse">The response type </typeparam>
    internal class AutoUnregisterEvent<TResponse> : IDisposable where TResponse : class
    {
        /// <summary> The transport to unregister from </summary>
        private readonly IMessageTransport<TResponse> transport;

        /// <summary> The receiver to detach. </summary>
        private readonly EventHandler<MessageReceivedEventArgs<TResponse>> receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUnregisterEvent{TResponse}"/> class.
        /// </summary>
        /// <param name="transport"> The transport. </param>
        /// <param name="receiver"> The receiver. </param>
        public AutoUnregisterEvent(
            IMessageTransport<TResponse> transport,
            EventHandler<MessageReceivedEventArgs<TResponse>> receiver)
        {
            this.transport = transport;
            this.receiver = receiver;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.transport.MessageReceived -= this.receiver;
        }
    }
}
