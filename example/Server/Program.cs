using System;
using Testing.Messages;

using MC.Messaging.Bus;
using MC.Messaging.Common;

namespace Messaging
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary> The client GUID </summary>
        private static readonly Guid ClientId = Guid.Parse("FF2150CF-39E8-4B9A-8C49-55CF9808DF27");

        /// <summary> The bus. </summary>
        private ICompositeServerMessageBus bus;

        private int pongCounter = 0;

        private DateTime start;

        /// <summary>
        /// The main method.
        /// </summary>
        public static void Main()
        {
            Program p = new Program();
            p.StartServerBus();

            bool cancelPressed = false;

            while (cancelPressed == false)
            {
                Console.WriteLine("\nPress <m> to send a single message, <p> to ping pong, <q> to quit");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                
                switch (keyInfo.KeyChar)
                {
                case 'p':
                    PingPong(p);
                    break;
                case 'q':
                    cancelPressed = true;
                    break;
                case 'm':
                    Console.WriteLine("\nSending Hello message to client");
                    p.bus.GetOrCreate(ClientId).Publish(new OneMessage { Message = "Hello from server" });
                    break;
                }
            }
        }

        /// <summary>
        /// Creates and starts a server message bus.
        /// </summary>
        public void StartServerBus()
        {
            this.bus = new CompositeServerMessageBus();

            this.bus.GetOrCreate(ClientId).Subscribe<OneResponse>(this.HandleResponse);
            this.bus.GetOrCreate(ClientId).Subscribe<Pong>(this.HandlePong);
        }

        private void StartPong()
        {
            this.start = DateTime.UtcNow;
            this.pongCounter = 0;
            PingPong(this);
        }

        /// <summary>
        /// Pings the client
        /// </summary>
        /// <param name="p">The program instance</param>
        private static void PingPong(Program p)
        {
            p.bus.GetOrCreate(ClientId).Publish(new Ping());
        }

        /// <summary>
        /// Handles the pong message, and sends a ping back
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args"> The arguments. </param>
        private void HandlePong(object sender, MessageReceivedEventArgs<Pong> args)
        {
            this.pongCounter++;
            if (this.pongCounter == 10000)
            {
                TimeSpan elapsed = DateTime.UtcNow - start;
                Console.WriteLine("PingPong - 10000 messages sent in {0}s", elapsed);
            }
            else
            {
                PingPong(this);
            }
        }

        /// <summary>
        /// The handle message.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args"> The message arguments </param>
        private void HandleResponse(object sender, MessageReceivedEventArgs<OneResponse> args)
        {
            Console.WriteLine("\nResponse received: {0}", args.Message.Response);
        }
    }
}
