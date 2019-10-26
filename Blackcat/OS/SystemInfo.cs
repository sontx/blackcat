using System.Text;

namespace Blackcat.OS
{
    public class OperatingSystem
    {
        public string Name { get; set; }
        public string Architecture { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Architecture}";
        }
    }

    public class Processor
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SystemInfo
    {
        public OperatingSystem OperatingSystem { get; set; }
        public Processor Processor { get; set; }
        public string MemorySize { get; set; }
        public string CurrentDiskFreeSpace { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("OS: " + OperatingSystem?.ToString());
            builder.AppendLine("Processor: " + Processor?.ToString());
            builder.AppendLine("RAM: " + MemorySize);
            builder.Append("Disk: " + CurrentDiskFreeSpace);

            return builder.ToString();
        }
    }
}