using Newtonsoft.Json;

namespace Blackcat.WinForm
{
    public partial class InputBox : ChooserForm
    {
        public static T Show<T>(string title, string description, T defaultValue)
        {
            using (var form = new InputBox { Text = title, Description = description })
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return form.GetValue<T>(defaultValue);
                }
            }
            return defaultValue;
        }

        private string description;

        public string Description
        {
            get => description;
            set
            {
                description = value;
                labDescription.Text = value;
            }
        }

        public object Value
        {
            get => txtInput.Text.Trim();
            set
            {
                txtInput.Text = value != null ? value.ToString() : "";
            }
        }

        public InputBox()
        {
            InitializeComponent();
        }

        public T GetValue<T>(T defaultValue)
        {
            try
            {
                var st = txtInput.Text.Trim() as object;
                if (typeof(T) == typeof(string))
                {
                    return (T)st;
                }
                return JsonConvert.DeserializeObject<T>(st.ToString());
            }
            catch { }
            return defaultValue;
        }
    }
}