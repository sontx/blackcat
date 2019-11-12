namespace Blackcat.WinForm
{
    public partial class SettingsForm : ChooserForm
    {
        private object settings;

        public object Settings
        {
            get => settings;
            set
            {
                settings = value;
                settingsPanel1.Settings = value;
                Text = settingsPanel1.Text;
            }
        }

        public bool SettingsChanged => settingsPanel1.SettingsChanged;

        public SettingsForm()
        {
            InitializeComponent();
            btnCancel.Visible = false;
            btnOK.Text = "Close";
        }
    }
}