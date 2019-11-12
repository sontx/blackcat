using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace Blackcat.EventBus
{
    internal class Subscriber
    {
        private readonly SubscribeAttribute _subscribeAttribute;
        private readonly MethodInfo _subscriberMethodInfo;
        private readonly Type _subscribeType;
        private readonly IThreadInvoker _threadInvoker;
        private readonly MessageChecker _messageChecker;

        public object Container { get; }

        public Subscriber(
            SubscribeAttribute subscribeAttribute,
            MethodInfo subscriberMethodInfo,
            IThreadInvoker threadInvoker,
            object container,
            MessageChecker messageChecker)
        {
            _subscribeAttribute = subscribeAttribute;
            _subscriberMethodInfo = subscriberMethodInfo;
            _threadInvoker = threadInvoker;
            Container = container;
            _messageChecker = messageChecker;

            var parameters = _subscriberMethodInfo.GetParameters();
            if (parameters.Length != 1)
                throw new ArgumentException("Subscriber method must have only one parameter.");
            _subscribeType = parameters[0].ParameterType;
        }

        public bool CanExecute(object message)
        {
            return _messageChecker.IsMatched(message, _subscribeType);
        }

        public object ExecuteIfNeeded(object message)
        {
            switch (_subscribeAttribute.ThreadMode)
            {
                case ThreadMode.Post:
                    return ExecuteSubscriber(Container, message);

                case ThreadMode.Thread:
                    ExecuteSubscriberInBackground(Container, message);
                    break;

                case ThreadMode.Async:
                    ExecuteSubscriberAsync(Container, message);
                    break;

                case ThreadMode.Main:
                    return ExecuteSubscriberInMain(Container, message);

                case ThreadMode.MainOrder:
                    ExecuteSubscriberInMainOrder(Container, message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
        }

        private void ExecuteSubscriberInMainOrder(object container, object message)
        {
            _threadInvoker.RunInMain(() => ExecuteSubscriber(container, message), false);
        }

        private object ExecuteSubscriberInMain(object container, object message)
        {
            object ret = null;
            _threadInvoker.RunInMain(() => ret = ExecuteSubscriber(container, message), true);
            return ret;
        }

        private void ExecuteSubscriberAsync(object container, object message)
        {
            Task.Run(() => ExecuteSubscriber(container, message));
        }

        private void ExecuteSubscriberInBackground(object container, object message)
        {
            if (_threadInvoker.IsMainThread())
                _threadInvoker.RunInBackground(() => ExecuteSubscriber(container, message));
            else
                ExecuteSubscriber(container, message);
        }

        private object ExecuteSubscriber(object container, object message)
        {
            var flags = BindingFlags.Instance | BindingFlags.InvokeMethod;
            var @params = new object[1] { message };
            return _subscriberMethodInfo.Invoke(container, flags, null, @params, CultureInfo.CurrentCulture);
        }
    }
}