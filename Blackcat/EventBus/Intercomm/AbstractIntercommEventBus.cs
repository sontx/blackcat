using System;
using System.Collections.Generic;

namespace Blackcat.EventBus.Intercomm
{
    public abstract class AbstractIntercommEventBus : IEventBus, IDisposable
    {
        protected readonly IEventBus internalEventBus;
        protected volatile bool disposed;

        public AbstractIntercommEventBus()
        {
            internalEventBus = new EventBus();
        }

        public object GetStickyEvent(Type eventType)
        {
            return internalEventBus.GetStickyEvent(eventType);
        }

        public virtual List<object> Post(object message, bool stick = false)
        {
            return internalEventBus.Post(message, stick);
        }

        public void Register(object container)
        {
            internalEventBus.Register(container);
        }

        public void RemoveAllStickyEvents()
        {
            internalEventBus.RemoveAllStickyEvents();
        }

        public void RemoveStickyEvent(object stickyEvent)
        {
            internalEventBus.RemoveStickyEvent(stickyEvent);
        }

        public void Unregister(object container)
        {
            internalEventBus.Unregister(container);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        ~AbstractIntercommEventBus()
        {
            Dispose(false);
        }
    }
}