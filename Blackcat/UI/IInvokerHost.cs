using System;

namespace Blackcat.UI
{
    public interface IInvokerHost
    {
        void Invoke(Action action);

        void BeginInvoke(Action action);
    }
}