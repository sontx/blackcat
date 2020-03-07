using System.IO;
using System.Reflection;

namespace Blackcat.Configuration
{
    public sealed class FileDataStorage : IDataStorage
    {
        private readonly object _lockObj = new object();
        private bool _disposed;
        private string _fileName;

        public  bool IsPresented { get; private set; }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                IsPresented = !string.IsNullOrEmpty(value) && File.Exists(value);
            }
        }

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
                lock (_lockObj)
                {
                    return File.ReadAllText(FileName);
                }
            }
            return "";
        }

        public void Save(string content, bool overwrite)
        {
            lock (_lockObj)
            {
                if (_disposed) return;

                if (overwrite || !File.Exists(FileName))
                {
                    File.WriteAllText(FileName, content);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_lockObj)
            {
                _disposed = true;
            }
        }
    }
}