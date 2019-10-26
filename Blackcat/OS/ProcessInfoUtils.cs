using System;
using System.Diagnostics;

namespace Blackcat.OS
{
    public static class ProcessInfoUtils
    {
        public static ProcessInfo GetProcessInfo()
        {
            var process = Process.GetCurrentProcess();
            var processInfo = new ProcessInfo
            {
                TotalMemoryUsed = $"{SystemInfoUtils.ByteToGB((ulong)process.WorkingSet64)} GB / {SystemInfoUtils.ByteToGB((ulong)process.PeakWorkingSet64)} GB",
                RunningTime = $"{DateTime.Now - process.StartTime}",
                CommandLine = Environment.CommandLine,
                ExecutablePath = process.MainModule.FileName,
                WorkingDirectory = Environment.CurrentDirectory
            };
            return processInfo;
        }
    }
}