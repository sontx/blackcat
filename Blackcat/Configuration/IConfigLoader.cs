using System;

namespace Blackcat.Configuration
{
    public interface IConfigLoader : IDisposable
    {
        SaveMode SaveMode { get; set; }
        IDataAdapter Adapter { get; set; }

        T Get<T>() where T : class;
    }
}