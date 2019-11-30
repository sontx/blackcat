using System;

namespace Blackcat.EventBus.Intercomm
{
    internal class EventWrapper
    {
        public object Data { get; set; }
        public Type DataType { get; set; }
        public bool Stick { get; set; }
    }
}