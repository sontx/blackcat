using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blackcat.EventBus
{
    public sealed class EventBus : IEventBus
    {
        private List<object> _containers = new List<object>();
        private IEnumerable<Subscriber> subscribers = new List<Subscriber>();
        private ConcurrentBag<object> stickedMessages = new ConcurrentBag<object>();
        private MatchedMode matchedMode;
        private MessageChecker messageChecker;
        private readonly object lockRegister = new object();
        private readonly object lockStickMessages = new object();

        public static EventBus Default { get; } = new EventBus();

        public IThreadInvoker ThreadInvoker { get; set; }

        public MatchedMode MatchedMode
        {
            get => matchedMode;
            set
            {
                matchedMode = value;
                messageChecker = new MessageChecker(value);
            }
        }

        public EventBus()
        {
            ThreadInvoker = UIThreadInvoker.IsSupported()
                ? (IThreadInvoker)new UIThreadInvoker()
                : new NonUIThreadInvoker();

            MatchedMode = MatchedMode.ExactlyType;
        }

        public void Register(object container)
        {
            var newSubscribers = GetSubscribers(container);

            lock (lockRegister)
            {
                if (_containers.Contains(container) || newSubscribers.Count == 0)
                    return;

                _containers.Add(container);

                var tempSubscribers = new List<Subscriber>(subscribers);
                tempSubscribers.AddRange(newSubscribers);
                subscribers = tempSubscribers;
            }

            BroadcastStickedMessages(newSubscribers);
        }

        private void BroadcastStickedMessages(IEnumerable<Subscriber> subscribers)
        {
            List<object> cloned;
            lock (lockStickMessages)
            {
                cloned = stickedMessages.ToList();
            }

            foreach (var message in cloned)
            {
                foreach (var subscriber in subscribers)
                {
                    if (subscriber.CanExecute(message))
                    {
                        subscriber.ExecuteIfNeeded(message);
                    }
                }
            }
        }

        public object GetStickyEvent(Type eventType)
        {
            lock (lockStickMessages)
            {
                foreach (var message in stickedMessages)
                {
                    if (messageChecker.IsMatched(message, eventType))
                    {
                        return message;
                    }
                }
            }
            return null;
        }

        public void RemoveStickyEvent(object stickyEvent)
        {
            if (stickyEvent == null) return;

            lock (lockStickMessages)
            {
                var remainingList = stickedMessages.Where(message => message != stickyEvent);
                stickedMessages = new ConcurrentBag<object>(remainingList);
            }
        }

        public void Unregister(object container)
        {
            lock (lockRegister)
            {
                if (_containers.Remove(container))
                {
                    var tempSubscribers = subscribers.Where(subscribe => subscribe.Container != container);
                    subscribers = tempSubscribers;
                }
            }
        }

        public List<object> Post(object message, bool stick = false)
        {
            if (message == null)
                return null;

            var executed = false;
            List<object> ret = null;
            foreach (var subscriber in subscribers)
            {
                if (!subscriber.CanExecute(message)) continue;
                executed = true;
                var result = subscriber.ExecuteIfNeeded(message);
                if (result is PostHandled)
                {
                    var handled = result as PostHandled;

                    if (ret == null)
                        ret = new List<object>();

                    if (handled.Data != null)
                        ret.Add(handled.Data);

                    if (handled.Canceled)
                        break;
                }
                else
                {
                    if (ret == null)
                        ret = new List<object>();

                    if (result != null)
                        ret.Add(result);
                }
            }

            if (!executed && stick)
            {
                AddToStickedMessageList(message);
            }

            return ret;
        }

        private void AddToStickedMessageList(object message)
        {
            var currentMatchedTypeEvent = GetStickyEvent(message.GetType());
            if (currentMatchedTypeEvent != null)
                RemoveStickyEvent(currentMatchedTypeEvent);
            
            lock (lockStickMessages)
            {
                stickedMessages.Add(message);
            }
        }

        private List<Subscriber> GetSubscribers(object container)
        {
            var runtimeMethods = container.GetType().GetRuntimeMethods();
            var subscribers = new List<Subscriber>();
            foreach (MethodInfo methodInfo in runtimeMethods)
            {
                var customAttribute = methodInfo.GetCustomAttribute<SubscribeAttribute>();
                if (customAttribute != null)
                {
                    var subscriber = new Subscriber(customAttribute, methodInfo, ThreadInvoker, container, messageChecker);
                    subscribers.Add(subscriber);
                }
            }
            return subscribers;
        }
    }
}