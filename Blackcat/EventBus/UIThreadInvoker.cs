using Blackcat.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blackcat.EventBus
{
    public class UIThreadInvoker : IThreadInvoker
    {
        private readonly int _mainThreadId;
        private readonly MethodInvoker invokeMethod;
        private readonly MethodInvoker beginInvokeMethod;
        private volatile bool _disponsed;

        public static bool IsSupported()
        {
            var type = DynamicInvoker.GetType("System.Windows.Forms", "Control");
            return type != null;
        }

        public UIThreadInvoker()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;

            var type = DynamicInvoker.GetType("System.Windows.Forms", "Control");
            var control = Activator.CreateInstance(type);
            DynamicInvoker.InvokeMethod(control, "CreateControl");
            invokeMethod = new MethodInvoker(control, "Invoke", new Type[] { typeof(Delegate) });
            beginInvokeMethod = new MethodInvoker(control, "BeginInvoke", new Type[] { typeof(Delegate) });
        }

        public bool IsMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == _mainThreadId;
        }

        public void RunInBackground(Action job)
        {
            Task.Run(job);
        }

        public void RunInMain(Action job, bool wait)
        {
            if (_disponsed) return;

            try
            {
                if (wait)
                    invokeMethod.Invoke(job);
                else
                    beginInvokeMethod.Invoke(job);
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    _disponsed = true;
                }
            }
        }
    }
}