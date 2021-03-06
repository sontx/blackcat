﻿using Blackcat.UI;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public partial class BaseForm : Form
    {
        public IInvokerHost InvokerHost { get; }
        public IMessageBoxHost MessageBoxHost { get; }

        public BaseForm()
        {
            InitializeComponent();
            InvokerHost = new InvokerHost(this);
            MessageBoxHost = new MessageBoxHost(this, InvokerHost);
        }
    }
}