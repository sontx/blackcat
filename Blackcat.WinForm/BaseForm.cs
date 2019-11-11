using System;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        public void Invoke(Action action)
        {
            try
            {
                Invoke((MethodInvoker)delegate { action?.Invoke(); });
            }
            catch { }
        }

        public void BeginInvoke(Action action)
        {
            try
            {
                BeginInvoke((MethodInvoker)delegate { action?.Invoke(); });
            }
            catch { }
        }

        public void ShowError(string msg)
        {
            MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowWarning(string msg)
        {
            MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowInfo(string msg)
        {
            MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool ShowQuestion(string msg, bool serious = false)
        {
            var icon = serious ? MessageBoxIcon.Question : MessageBoxIcon.Exclamation;
            return MessageBox.Show(msg, Text, MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }
    }
}