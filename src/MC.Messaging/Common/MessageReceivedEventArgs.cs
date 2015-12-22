using System;

namespace MC.Messaging.Common
{
    /// <summary>
    /// An event arguments class, containing a single received message.
    /// </summary>
    /// <typeparam name="T">The type of message </typeparam>
    public class MessageReceivedEventArgs<T> : EventArgs where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs{T}"/> class. 
        /// </summary>
        /// <param name="message"> The received message </param>
        public MessageReceivedEventArgs(T message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public T Message { get; private set; }
    }
}
