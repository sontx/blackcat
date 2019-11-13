using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Blackcat.Configuration
{
    public sealed class FileDataStorage : IDataStorage
    {
        private readonly ManualResetEvent waitRead = new ManualResetEvent(true);
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
                waitRead.WaitOne();
                return File.ReadAllText(FileName);
            }
            return "";
        }

        public void Save(string content)
        {
            try
            {
                waitRead.WaitOne();
                waitRead.Reset();
                File.WriteAllText(FileName, content);
            }
            finally
            {
                waitRead.Set();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                waitRead.Close();
                GC.SuppressFinalize(this);
            }
        }
    }
}