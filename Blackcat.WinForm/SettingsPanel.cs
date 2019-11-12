using Blackcat.Configuration;
using Blackcat.Types;
using Newtonsoft.Json;
using System.Reflection;
using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public partial class SettingsPanel : UserControl
    {
        private object settings;
        private string originalSettings;

        public object Settings
        {
            get => settings;
            set
            {
                settings = value;

                propertyGrid1.SelectedObject = settings;
                labelDescription.Visible = false;

                originalSettings = JsonConvert.SerializeObject(settings);

                if (settings != null)
                {
                    var configClassAttr = value.GetType().GetCustomAttribute<ConfigClassAttribute>();
                    labelDescription.Visible = !string.IsNullOrEmpty(configClassAttr.Description);
                    labelDescription.Text = configClassAttr.Description;
                    Text = configClassAttr.Key.SplitCamelCase();
                }
            }
        }

        public bool SettingsChanged => originalSettings != JsonConvert.SerializeObject(settings);

        public SettingsPanel()
        {
            InitializeComponent();
        }
    }
}