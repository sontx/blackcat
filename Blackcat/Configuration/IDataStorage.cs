using System;

namespace Blackcat.Configuration
{
    public interface IDataStorage : IDisposable
    {
        bool IsPresented { get; }

        void Save(string content, bool overwrite);

        string Load();
    }
}