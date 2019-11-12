using Blackcat.UI;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public sealed class MessageBoxHost : IMessageBoxHost
    {
        private readonly Control hostControl;
        private readonly IInvokerHost invokerHost;

        public MessageBoxHost(Control hostControl, IInvokerHost invokerHost)
        {
            this.hostControl = hostControl;
            this.invokerHost = invokerHost;
        }

        public void ShowError(string msg)
        {
            invokerHost.Invoke(() => MessageBox.Show(msg, hostControl.Text, MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        public void ShowWarning(string msg)
        {
            invokerHost.Invoke(() => MessageBox.Show(msg, hostControl.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning));
        }

        public void ShowInfo(string msg)
        {
            invokerHost.Invoke(() => MessageBox.Show(msg, hostControl.Text, MessageBoxButtons.OK, MessageBoxIcon.Information));
        }

        public bool ShowQuestion(string msg, bool serious = false)
        {
            var icon = serious ? MessageBoxIcon.Question : MessageBoxIcon.Exclamation;
            var ret = false;
            invokerHost.Invoke(() =>
            {
                ret = MessageBox.Show(msg, hostControl.Text, MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
            });
            return ret;
        }
    }
}