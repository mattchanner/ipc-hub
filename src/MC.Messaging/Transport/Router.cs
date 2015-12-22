using System;
using System.Collections.Generic;
using System.Linq;

namespace MC.Messaging.Transport
{   
    /// <summary>
    /// A message router to route message to specific handlers
    /// </summary>
    public class Router
    {
        /// <summary> The route table. </summary>
        private IDictionary<Type, List<object>> routeTable = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Adds a handler for messages of type T
        /// </summary>
        /// <typeparam name="T">The type of message to route.</typeparam>
        /// <param name="handler">The handler to route messages of type T to.</param>
        public void AddMessageRouter<T>(Action<T> handler) where T : class
        {
            this.UnregisterHandler(handler);

            List<object> handlers;

            if (!this.routeTable.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<object>();
                this.routeTable.Add(typeof(T), handlers);
            }

            handlers.Add(handler);
        }

        /// <summary>
        /// Unregisters the given handler from the route
        /// </summary>
        /// <param name="handler"> The handler to unregister. </param>
        /// <typeparam name="T"> The type of message to unregister from </typeparam>
        public void UnregisterHandler<T>(Action<T> handler)
        {
            List<object> handlers;

            if (this.routeTable.TryGetValue(typeof(T), out handlers))
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Routes the message to the relevant handler
        /// </summary>
        /// <param name="message"> The message to route. </param>
        /// <typeparam name="T"> The type of message to route </typeparam>
        public void Route<T>(T message) where T : class
        {
            List<object> handlers;

            if (this.routeTable.TryGetValue(typeof(T), out handlers))
            {
                foreach (Action<T> action in handlers.OfType<Action<T>>())
                {
                    action(message);
                }
            }
        }

        /// <summary>
        /// Routes the message to the relevant handler
        /// </summary>
        /// <param name="message"> The message to route. </param>
        public void Route(object message)
        {
            List<object> handlers;

            if (this.routeTable.TryGetValue(message.GetType(), out handlers))
            {
                foreach (MulticastDelegate delegateReference in handlers.Cast<MulticastDelegate>())
                {
                    delegateReference.DynamicInvoke(new[] { message });    
                }
            }
        }
    }
}
