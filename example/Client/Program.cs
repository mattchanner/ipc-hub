using System;

using Testing.Messages;

using MC.Messaging.Bus;
using MC.Messaging.Common;

namespace Client
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary> The bus. </summary>
        private IClientMessageBus bus;

        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            Program p = new Program();
            p.StartClientBus();

            Console.ReadKey();
        }

        /// <summary>
        /// Creates and starts a client message bus.
        /// </summary>
        public void StartClientBus()
        {
            Guid clientId = Guid.Parse("FF2150CF-39E8-4B9A-8C49-55CF9808DF27");

            this.bus = new ClientMessageBus(clientId);
            this.bus.Subscribe<OneMessage>(this.HandleMessage);
            this.bus.Subscribe<Ping>(this.HandlePing);
        }

        /// <summary>
        /// The handle message.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args"> The message arguments </param>
        private void HandleMessage(object sender, MessageReceivedEventArgs<OneMessage> args)
        {
            Console.WriteLine("\nReceived message: {0}", args.Message.Message);
            Console.WriteLine("Sending echo message back to the server");
            this.bus.Publish(new OneResponse { Response = "Echo echo echo" });
        }

        /// <summary>
        /// The handle message.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args"> The message arguments </param>
        private void HandlePing(object sender, MessageReceivedEventArgs<Ping> args)
        {
            this.bus.Publish(new Pong());
        }
    }
}
