using Blackcat.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Blackcat.OS
{
    public sealed class AppCrash
    {
        private Type _reportWindow;

        public string ProductName { get; set; }
        public string DeveloperMail { get; set; }
        public string CrashDir { get; set; }

        public AppCrash()
        {
            var applicationType = DynamicInvoker.GetType("System.Windows.Forms", "Application");
            if (applicationType != null)
                DynamicInvoker.AddEventHandler<ThreadExceptionEventHandler>(applicationType, "ThreadException", Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ProductName = DynamicInvoker.GetPropertyValue(applicationType, "ProductName") as string;
            DeveloperMail = "unknown@mail.com";
            CrashDir = "CrashLogs";
        }

        public void Register(Type reportWindow)
        {
            _reportWindow = reportWindow;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                ShowReport(exception);
            }
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                ShowReport(e.Exception);
            }
        }

        private void ShowReport(Exception exception)
        {
            var file = SaveToDisk(exception);
            if (_reportWindow != null)
            {
                var reportWindow = (IReportWindow)Activator.CreateInstance(_reportWindow);
                reportWindow.Initialize(exception, ProductName, DeveloperMail);
                reportWindow.Show();
            }
            else
            {
                Process.Start("notepad.exe", file);
            }
        }

        private string SaveToDisk(Exception exception)
        {
            var crashDir = string.IsNullOrEmpty(CrashDir)
                ? Path.Combine(Environment.CurrentDirectory, "CrashLogs")
                : CrashDir;

            if (!Directory.Exists(crashDir))
                Directory.CreateDirectory(crashDir);

            var fileName = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.txt";
            var filePath = Path.Combine(crashDir, fileName);

            var report = GenerateReport(exception, ProductName);
            File.WriteAllText(filePath, report, Encoding.UTF8);

            return filePath;
        }

        public string GenerateReport(Exception exception, string productName)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Application crashed report".ToUpper());
            builder.AppendLine();
            builder.AppendLine($"Product: {productName}");
            builder.AppendLine($"Report date: {DateTime.Now}");
            builder.AppendLine($"-----------------------------------------------");
            builder.AppendLine($"System Information");
            builder.AppendLine(SystemInfoUtils.GetSystemInfo().ToString());
            builder.AppendLine($"-----------------------------------------------");
            builder.AppendLine($"Process Information");
            builder.AppendLine(ProcessInfoUtils.GetProcessInfo().ToString());
            builder.AppendLine($"-----------------------------------------------");
            builder.AppendLine($"Exception");
            builder.Append(exception);
            return builder.ToString();
        }
    }
}