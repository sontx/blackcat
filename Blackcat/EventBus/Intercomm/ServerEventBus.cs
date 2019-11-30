using Blackcat.Intercomm;
using Blackcat.Intercomm.Pipe;
using Blackcat.Internal;
using System.Threading;

namespace Blackcat.EventBus.Intercomm
{
    /// <summary>
    /// <see cref="IEventBus"/> communicating between two processes, this should be used in the main process
    /// or a persistent process such as Window services, main application...
    /// </summary>
    public sealed class ServerEventBus : AbstractIntercommEventBus
    {
        /// <summary>
        /// Create a simple <see cref="ServerEventBus"/> with pre-configuring and ready to use.
        /// </summary>
        /// <param name="name">A name to identify this process, this name should be the same as <see cref="ClientEventBus"/></param>
        /// <returns>A ready to use <see cref="ServerEventBus"/></returns>
        public static ServerEventBus StartNew(string name)
        {
            var eventbus = new ServerEventBus();
            eventbus.Receiver = new SingleReceiver(name);
            eventbus.Start();
            return eventbus;
        }

        private readonly Thread thread;

        /// <summary>
        /// Underlying class which helps to communicate with the other side.
        /// </summary>
        public ISingleReceiver Receiver { get; set; }

        public ServerEventBus()
        {
            thread = new Thread(WaitForIncommingData);
            thread.IsBackground = true;
        }

        /// <summary>
        /// Starts waiting for incomming events from the other side.
        /// </summary>
        public void Start()
        {
            thread.Start();
        }

        private void WaitForIncommingData()
        {
            var receiver = Receiver;
            Precondition.PropertyNotNull(receiver, nameof(Receiver));

            var adapter = new JsonContentAdapter();

            while (!disposed)
            {
                var data = receiver.ReceiveAsync<EventWrapper>().Result;
                var decodedData = adapter.ToObject(data.Data.ToString(), data.DataType);
                internalEventBus.Post(decodedData, data.Stick);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Receiver?.Dispose();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}