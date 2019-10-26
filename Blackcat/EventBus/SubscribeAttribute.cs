using System;

namespace Blackcat.EventBus
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
        public SubscribeAttribute(ThreadMode mode = ThreadMode.Post)
        {
            ThreadMode = mode;
        }

        public ThreadMode ThreadMode { get; set; } = ThreadMode.Post;
    }
}