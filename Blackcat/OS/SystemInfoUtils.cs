using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.IO;

namespace Blackcat.OS
{
    public static class SystemInfoUtils
    {
        public static OperatingSystem GetOperatingSystem()
        {
            var computerInfo = new ComputerInfo();
            return new OperatingSystem
            {
                Name = computerInfo.OSFullName,
                Architecture = Environment.Is64BitOperatingSystem ? "64Bit" : "32Bit"
            };
        }

        public static Processor GetProcessor()
        {
            var processor = new Processor();
            using (var key = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree))
            {
                processor.Name = key.GetValue("ProcessorNameString", "Unknown")?.ToString();
            }
            return processor;
        }

        public static SystemInfo GetSystemInfo()
        {
            var systemInfo = new SystemInfo();
            systemInfo.OperatingSystem = GetOperatingSystem();
            systemInfo.Processor = GetProcessor();

            var computerInfo = new ComputerInfo();
            var totalRam = computerInfo.TotalPhysicalMemory;
            var freeRam = computerInfo.AvailablePhysicalMemory;
            systemInfo.MemorySize = $"{ByteToGB(freeRam)} GB / {ByteToGB(totalRam)} GB";

            var computer = new Computer();
            var currentDisk = Path.GetFullPath(Environment.CurrentDirectory).Substring(0, 1);
            var diskInfo = computer.FileSystem.GetDriveInfo(currentDisk);
            systemInfo.CurrentDiskFreeSpace = diskInfo != null
                ? $"{ByteToGB((ulong)diskInfo.AvailableFreeSpace)} GB / {ByteToGB((ulong)diskInfo.TotalSize)} GB"
                : "Unknown";

            return systemInfo;
        }

        internal static string ByteToGB(ulong bytes)
        {
            var total = Convert.ToDouble(bytes / (1024D * 1024D * 1024D));
            return total.ToString("F1");
        }
    }
}