using System;
using System.Diagnostics;
using System.IO.Pipes;

namespace MC.Messaging.Transport.Pipes
{
    /// <summary>
    /// Represents a client pipe
    /// </summary>
    public class NamedPipeClient : AbstractNamedPipe<NamedPipeClientStream>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeClient"/> class. 
        /// </summary>
        /// <param name="pipeName"> The name of the pipe </param>
        /// <param name="dispatcher">The message dispatcher </param>
        public NamedPipeClient(string pipeName, MessageDispatcher dispatcher)
            : base(pipeName, dispatcher)
        {
        }

        /// <summary>
        /// Creates the client stream
        /// </summary>
        /// <returns>
        /// The <see cref="NamedPipeClientStream"/>. </returns>
        protected override NamedPipeClientStream CreateStream()
        {
            var stream = new NamedPipeClientStream(
                ".",
                PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            stream.Connect();
            stream.ReadMode = PipeTransmissionMode.Message;
            return stream;
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
                    if (Pipe.IsConnected)
                    {
                        byte[] msg = ReadMessage(Pipe);

                        if (msg != null && msg.Length > 0)
                        {
                            OnReceivedMessage(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                Debug.WriteLine("Client.Run() is exiting.");
            }
        }
    }
}
