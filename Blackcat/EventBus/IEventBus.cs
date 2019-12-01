using System;
using System.Collections.Generic;

namespace Blackcat.EventBus
{
    /// <summary>
    /// Lightweight event aggregator/messenger for loosely coupled communication
    /// </summary>
    public interface IEventBus
    {
        void Register(object container);

        void Unregister(object container);

        List<object> Post(object message, bool stick = false);

        object GetStickyEvent(Type eventType);

        void RemoveStickyEvent(object stickyEvent);

        void RemoveAllStickyEvents();
    }
}