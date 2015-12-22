using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace MC.Messaging.Transport.Pipes
{
    /// <summary>
    /// The abstract named pipe class
    /// </summary>
    /// <typeparam name="T"> The pipe stream to manage </typeparam>
    public abstract class AbstractNamedPipe<T> : IDisposable where T : PipeStream
    {
        /// <summary> The buffer size. </summary>
        protected const int BufferSize = 0x1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractNamedPipe{T}"/> class.
        /// </summary>
        /// <param name="pipeName"> The pipe name. </param>
        /// <param name="dispatcher">The dispatcher </param>
        /// <exception cref="ArgumentNullException">Raised when the pipe name is null. </exception>
        protected AbstractNamedPipe(string pipeName, MessageDispatcher dispatcher)
        {
            if (pipeName == null)
            {
                throw new ArgumentNullException("pipeName", "Argument cannot be null.");
            }

            this.Dispatcher = dispatcher;
            this.PipeName = pipeName;
            this.Router = new Router();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AbstractNamedPipe{T}"/> class. 
        /// </summary>
        ~AbstractNamedPipe()
        {
            this.Dispose(false);
        }
        
        /// <summary>
        /// Gets the router.
        /// </summary>
        public Router Router
        {
            get; private set;
        }

        /// <summary>
        /// Gets the pipe name.
        /// </summary>
        public string PipeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the dispatcher
        /// </summary>
        protected MessageDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets or sets the instance of PipeStream.
        /// </summary>
        protected T Pipe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stop has been .
        /// </summary>
        protected bool StopRequested { get; set; }

        /// <summary>
        /// Create the named pipe stream and starts the reader thread.
        /// </summary>
        public void Start()
        {
            this.Pipe = this.CreateStream();
            ThreadPool.QueueUserWorkItem(this.ReadFromPipe);
        }

        /// <summary>
        /// Requests the reader thread stop and disposes the named pipe.
        /// </summary>
        public void Stop()
        {
            this.Dispose();
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Write a message to the pipe if the pipe is connected.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to read </typeparam>
        /// <param name="message"> The message to write </param>
        public void Write<TMessage>(TMessage message) where TMessage : class
        {
            if (this.Pipe.IsConnected && this.Pipe.CanWrite)
            {
                this.WriteToStream(message);
            }
        }

        /// <summary>
        /// Reads a message from the pipe.
        /// </summary>
        /// <param name="stream">The message to read</param>
        /// <returns>The read message</returns>
        protected static byte[] ReadMessage(PipeStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] buffer = new byte[BufferSize];

            do
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, bytesRead);
            }
            while (stream.IsMessageComplete == false);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Creates an instance of T.
        /// </summary>
        /// <returns>The created stream</returns>
        protected abstract T CreateStream();

        /// <summary>
        /// Method that runs on the ThreadPool for reading messages from the pipe.
        /// </summary>
        /// <param name="state">The object state</param>
        protected abstract void ReadFromPipe(object state);

        /// <summary>
        /// Write a message to the pipe's stream.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to write </typeparam>
        /// <param name="message"> The message to write to the stream </param>
        protected virtual void WriteToStream<TMessage>(TMessage message) where TMessage : class
        {
            this.Dispatcher.WriteMessage(message, this.Pipe);
            this.Pipe.WaitForPipeDrain();
        }

        /// <summary>
        /// Fire the OnReceivedMessage event.
        /// </summary>
        /// <param name="message">The message sent with the event.</param>
        protected void OnReceivedMessage(byte[] message)
        {
            MemoryStream memoryStream  = new MemoryStream(message);
            memoryStream.Position = 0;
            object response = this.Dispatcher.ReadMessage(memoryStream);
            this.Router.Route(response);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"> True if called from Dispose. </param>
        protected virtual void Dispose(bool disposing)
        {
            this.StopRequested = true;

            try
            {
                this.Pipe.Close();
                this.Pipe.Dispose();    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);   
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
