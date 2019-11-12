using Blackcat.Configuration;
using Blackcat.Types;
using Newtonsoft.Json;
using System.Reflection;

namespace Blackcat.WinForm
{
    public partial class SettingsPanel : BaseUserControl
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
                    var settingsType = value.GetType();
                    var configClassAttr = settingsType.GetCustomAttribute<ConfigClassAttribute>();
                    if (configClassAttr != null)
                    {
                        labelDescription.Visible = !string.IsNullOrEmpty(configClassAttr.Description);
                        labelDescription.Text = configClassAttr.Description;
                        var key = configClassAttr.Key;
                        Text = (string.IsNullOrEmpty(key) ? settingsType.Name : key).SplitCamelCase();
                    }
                    else
                    {
                        Text = settingsType.Name.SplitCamelCase();
                    }
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