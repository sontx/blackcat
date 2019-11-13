using System;

namespace Blackcat.Configuration
{
    public interface IDataStorage : IDisposable
    {
        void Save(string content);

        string Load();
    }
}