using System;
using System.IO.Pipes;

namespace MC.Messaging.Transport.Pipes
{
    /// <summary>
    /// Represents the server pipe
    /// </summary>
    public class NamedPipeServer : AbstractNamedPipe<NamedPipeServerStream>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer"/> class.
        /// </summary>
        /// <param name="pipeName"> The pipe name. </param>
        /// <param name="dispatcher">The message dispatcher </param>
        public NamedPipeServer(string pipeName, MessageDispatcher dispatcher)
            : base(pipeName, dispatcher)
        {
        }

        /// <summary>
        /// Creates an instance of T.
        /// </summary>
        /// <returns>The created stream</returns>
        protected override NamedPipeServerStream CreateStream()
        {
            return new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                BufferSize,
                BufferSize);
        }

        /// <summary>
        /// Method that runs on the ThreadPool for reading messages from the pipe.
        /// </summary>
        /// <param name="state">The object state</param>
        protected override void ReadFromPipe(object state)
        {
            try
            {
                while (Pipe != null && StopRequested == false)
                {
                    if (Pipe.IsConnected == false)
                    {
                        Pipe.WaitForConnection();
                    }

                    byte[] msg = ReadMessage(Pipe);

                    if (msg != null && msg.Length > 0)
                    {
                        OnReceivedMessage(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
