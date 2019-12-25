using System.IO;
using System.Reflection;

namespace Blackcat.Configuration
{
    public sealed class FileDataStorage : IDataStorage
    {
        private readonly object lockObj = new object();
        private bool disposed;

        public string FileName { get; set; }

        public FileDataStorage()
        {
            FileName = GetDefaultConfigFileName();
        }

        private string GetDefaultConfigFileName()
        {
            var assembly = Assembly.GetEntryAssembly() ?? GetType().Assembly;
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(assembly.Location);
            return $"{nameWithoutExtension}.json";
        }

        public string Load()
        {
            if (File.Exists(FileName))
            {
                lock (lockObj)
                {
                    return File.ReadAllText(FileName);
                }
            }
            return "";
        }

        public void Save(string content, bool overwrite)
        {
            lock (lockObj)
            {
                if (!disposed)
                {
                    if (overwrite || !File.Exists(FileName))
                    {
                        File.WriteAllText(FileName, content);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                lock (lockObj)
                {
                    disposed = true;
                }
            }
        }
    }
}