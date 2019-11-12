using Blackcat.UI;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public partial class BaseUserControl : UserControl
    {
        public IInvokerHost InvokerHost { get; }
        public IMessageBoxHost MessageBoxHost { get; }

        public BaseUserControl()
        {
            InitializeComponent();
            InvokerHost = new InvokerHost(this);
            MessageBoxHost = new MessageBoxHost(this, InvokerHost);
        }
    }
}