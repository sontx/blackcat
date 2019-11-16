using System.Windows.Forms;

namespace Blackcat.WinForm
{
    public static class Extension
    {
        public static bool ShowEditor(this object obj, bool showModal, Form owner)
        {
            using (var form = new SettingsForm { Settings = obj })
            {
                if (showModal)
                    form.ShowDialog(owner);
                else
                    form.Show();
                return form.SettingsChanged;
            }
        }

        public static bool ShowEditor(this object obj, bool showModal = true)
        {
            return ShowEditor(obj, showModal, null);
        }

        public static bool ShowEditor(this object obj, Form owner)
        {
            return ShowEditor(obj, true, owner);
        }
    }
}