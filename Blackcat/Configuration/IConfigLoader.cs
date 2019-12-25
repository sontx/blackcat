using System;

namespace Blackcat.Configuration
{
    public interface IConfigLoader : IDisposable
    {
        /// <summary>
        /// When changes will be saved
        /// </summary>
        SaveMode SaveMode { get; set; }

        /// <summary>
        /// How changes will be encoded or decoded
        /// </summary>
        IDataAdapter Adapter { get; set; }

        /// <summary>
        /// How encoded changes will be saved
        /// </summary>
        IDataStorage Storage { get; set; }

        /// <summary>
        /// Setups settings if it's not presented yet.
        /// </summary>
        void InitializeSettings(object[] settings);

        /// <summary>
        /// Gets configuration instance by its type
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        T Get<T>() where T : class;
    }
}