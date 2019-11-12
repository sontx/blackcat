using Blackcat.UI;
using System;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public sealed class InvokerHost : IInvokerHost
    {
        private readonly Control hostControl;

        public InvokerHost(Control hostControl)
        {
            this.hostControl = hostControl;
        }

        public void BeginInvoke(Action action)
        {
            if (hostControl.InvokeRequired)
            {
                try
                {
                    hostControl.BeginInvoke((MethodInvoker)delegate
                    {
                        BeginInvoke(action);
                    });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                action?.Invoke();
            }
        }

        public void Invoke(Action action)
        {
            if (hostControl.InvokeRequired)
            {
                try
                {
                    hostControl.Invoke((MethodInvoker)delegate
                    {
                        BeginInvoke(action);
                    });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                action?.Invoke();
            }
        }
    }
}