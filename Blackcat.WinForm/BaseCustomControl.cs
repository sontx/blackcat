using Blackcat.UI;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public class BaseCustomControl : Control
    {
        public IInvokerHost InvokerHost { get; }
        public IMessageBoxHost MessageBoxHost { get; }

        public BaseCustomControl()
        {
            InvokerHost = new InvokerHost(this);
            MessageBoxHost = new MessageBoxHost(this, InvokerHost);
        }
    }
}