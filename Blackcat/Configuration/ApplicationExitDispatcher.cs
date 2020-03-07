using Blackcat.Utils;
using System;

namespace Blackcat.Configuration
{
    public class ApplicationExitDispatcher : IApplicationExitDispatcher
    {
        public event EventHandler Exit;

        public ApplicationExitDispatcher()
        {
            var type = DynamicInvoker.GetType("System.Windows.Forms", "Application");
            if (type != null)
            {
                DynamicInvoker.AddEventHandler<EventHandler>(type, "ApplicationExit", Application_ApplicationExit);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Exit?.Invoke(sender, e);
        }
    }
}