using System;

namespace Blackcat.WinForm
{
    public partial class ChooserForm : VerticalLayoutForm
    {
        public ChooserForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (OnAccept())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        protected virtual bool OnAccept()
        {
            return true;
        }
    }
}