using Blackcat.Intercomm;
using Blackcat.Intercomm.Pipe;
using System.Collections.Generic;

namespace Blackcat.EventBus.Intercomm
{
    /// <summary>
    /// <see cref="IEventBus"/> communicating between two processes.
    /// </summary>
    public sealed class ClientEventBus : AbstractIntercommEventBus
    {
        /// <summary>
        /// Create a simple <see cref="ClientEventBus"/> with pre-configuring and ready to use.
        /// </summary>
        /// <param name="name">A name to identify this process, this name should be the same as <see cref="ServerEventBus"/></param>
        /// <returns>A ready to use <see cref="ClientEventBus"/></returns>
        public static ClientEventBus StartNew(string name)
        {
            var eventbus = new ClientEventBus();
            eventbus.Sender = new Sender(name);
            return eventbus;
        }

        /// <summary>
        /// Underlying class which helps to communicate with the other side.
        /// </summary>
        public ISender Sender { get; set; }

        /// <summary>
        /// Note: Response data from the other side is not supported
        /// </summary>
        public override List<object> Post(object message, bool stick = false)
        {
            if (message != null)
            {
                Sender.SendAsync(new EventWrapper
                {
                    Data = message,
                    DataType = message.GetType(),
                    Stick = stick
                });
            }

            return base.Post(message, stick);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Sender?.Dispose();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}