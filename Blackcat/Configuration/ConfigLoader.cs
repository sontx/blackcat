using Blackcat.Configuration.AutoNotifyPropertyChange;
using Blackcat.Types;
using Blackcat.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blackcat.Configuration
{
    public sealed class ConfigLoader : IConfigLoader
    {
        public static ConfigLoader Default { get; } = new ConfigLoader();

        private readonly ConcurrentDictionary<string, object> configDict = new ConcurrentDictionary<string, object>();
        private readonly object lockLoadIndividualConfig = new object();

        private SaveMode saveMode;
        private bool disposed;

        public SaveMode SaveMode
        {
            get => saveMode;
            set
            {
                if (saveMode == value) return;

                if (value == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(configDict);
                    foreach (var data in dict.Values)
                    {
                        SubscribeChanges(data);
                    }
                }
                else if (value == SaveMode.ReadOnly && saveMode == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(configDict);
                    foreach (var data in dict.Values)
                    {
                        UnsubscribeChanges(data);
                    }
                }
                saveMode = value;
            }
        }

        public IDataAdapter Adapter { get; set; } = new JsonDataAdapter();

        public IDataStorage Storage { get; set; } = new FileDataStorage();

        public ConfigLoader()
        {
            var type = DynamicInvoker.GetType("System.Windows.Forms", "Application");
            if (type != null)
            {
                DynamicInvoker.AddEventHandler<EventHandler>(type, "ApplicationExit", Application_ApplicationExit);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (SaveMode != SaveMode.ReadOnly)
            {
                SaveConfigToFile();
            }
        }

        private void SaveConfigToFile()
        {
            if (SaveMode == SaveMode.ReadOnly)
                return;

            var dict = new Dictionary<string, object>(configDict);
            var configs = dict.Select(pair => new ConfigElement
            {
                Key = pair.Key,
                Data = pair.Value
            }).ToList();

            var configFile = new ConfigFile
            {
                Metadata = new Metadata
                {
                    Modified = DateTime.Now
                },
                Configs = configs
            };

            var contentToSave = Adapter.ToString(configFile);
            Storage.Save(contentToSave);
        }

        public T Get<T>() where T : class
        {
            var requestType = typeof(T);
            var requestKey = GetRequestKey(requestType);

            lock (lockLoadIndividualConfig)
            {
                if (!configDict.ContainsKey(requestKey))
                    LoadConfig<T>(requestKey);
            }

            if (configDict.TryGetValue(requestKey, out var ret))
                return (T)ret;
            return null;
        }

        private string GetRequestKey(Type requestType)
        {
            var configAttr = requestType.GetCustomAttribute<ConfigClassAttribute>();
            if (configAttr == null || string.IsNullOrEmpty(configAttr.Key))
            {
                return requestType.Name;
            }
            return configAttr.Key;
        }

        private void LoadConfig<T>(string requestKey) where T : class
        {
            var configFile = GetConfigFile();
            var found = configFile.Configs.Find(config => string.Compare(requestKey, config.Key) == 0);
            if (found != null)
            {
                var data = Adapter.ToObject<T>(found.Data);
                configDict.TryAdd(requestKey, PreprocessLoadedData(data));
            }
            else
            {
                var newConfigInstance = Activator.CreateInstance<T>();
                configDict.TryAdd(requestKey, PreprocessLoadedData(newConfigInstance));
            }
        }

        private ConfigFile GetConfigFile()
        {
            var content = Storage.Load();
            return string.IsNullOrEmpty(content)
                ? ConfigFile.Empty
                : Adapter.ToObject<ConfigFile>(content);
        }

        private T PreprocessLoadedData<T>(T loadedData) where T : class
        {
            if (loadedData is AutoNotifyPropertyChanged)
            {
                var decoratedData = AutoNotifyPropertyChanged.CreateInstance<T>();
                decoratedData.Populate(loadedData);
                if (SaveMode == SaveMode.OnChange)
                {
                    SubscribeChanges(decoratedData);
                }
                return decoratedData;
            }
            return loadedData;
        }

        private void SubscribeChanges(object data)
        {
            if (data is AutoNotifyPropertyChanged propChanged)
            {
                propChanged.PropertyChanged -= Data_PropertyChanged;
                propChanged.PropertyChanged += Data_PropertyChanged;

                var props = data.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (typeof(AutoNotifyPropertyChanged).IsAssignableFrom(prop.PropertyType))
                    {
                        SubscribeChanges(prop.GetValue(data));
                    }
                }
            }
        }

        private void UnsubscribeChanges(object data)
        {
            if (data is AutoNotifyPropertyChanged propChanged)
            {
                propChanged.PropertyChanged -= Data_PropertyChanged;
            }
        }

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Task.Run(SaveConfigToFile);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Storage?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}