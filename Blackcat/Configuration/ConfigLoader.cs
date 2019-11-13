using Blackcat.Configuration.AutoNotifyPropertyChange;
using Blackcat.Types;
using Blackcat.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blackcat.Configuration
{
    public class ConfigLoader : IConfigLoader
    {
        public static ConfigLoader Default { get; } = new ConfigLoader();

        private readonly ConcurrentDictionary<string, object> loadedConfigDict = new ConcurrentDictionary<string, object>();
        private readonly object lockLoadIndividualConfig = new object();
        private readonly object lockLoadConfigFile = new object();
        private ConfigFile loadedConfigFile;
        private string loadedFileName;
        private SaveMode saveMode;

        public SaveMode SaveMode
        {
            get => saveMode;
            set
            {
                if (saveMode == value) return;

                if (value == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(loadedConfigDict);
                    foreach (var data in dict.Values)
                    {
                        SubscribeChanges(data);
                    }
                }
                else if (value == SaveMode.ReadOnly && saveMode == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(loadedConfigDict);
                    foreach (var data in dict.Values)
                    {
                        UnsubscribeChanges(data);
                    }
                }
                saveMode = value;
            }
        }

        public IDataAdapter Adapter { get; set; } = new JsonDataAdapter();

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

        public void SaveConfigToFile()
        {
            if (SaveMode == SaveMode.ReadOnly)
                return;

            var dict = new Dictionary<string, object>(loadedConfigDict);
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
            File.WriteAllText(loadedFileName, contentToSave);
        }

        public T Get<T>() where T : class
        {
            lock (lockLoadConfigFile)
            {
                if (loadedConfigFile == null)
                {
                    LoadFile();
                }
            }

            var requestType = typeof(T);
            var requestKey = GetRequestKey(requestType);

            lock (lockLoadIndividualConfig)
            {
                if (!loadedConfigDict.ContainsKey(requestKey))
                    LoadConfig<T>(requestKey);
            }

            if (loadedConfigDict.TryGetValue(requestKey, out var ret))
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
            var found = loadedConfigFile?.Configs.Find(config => string.Compare(requestKey, config.Key) == 0);
            if (found != null)
            {
                var data = Adapter.ToObject<T>(found.Data);
                loadedConfigDict.TryAdd(requestKey, PreprocessLoadedData(data));
            }
            else
            {
                var newConfigInstance = Activator.CreateInstance<T>();
                loadedConfigDict.TryAdd(requestKey, PreprocessLoadedData(newConfigInstance));
            }
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

        private void LoadFile(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = GetDefaultConfigFileName();

            loadedFileName = fileName;

            if (!File.Exists(fileName))
            {
                loadedConfigFile = new ConfigFile { Configs = new List<ConfigElement>(0) };
                return;
            }

            var textContent = File.ReadAllText(fileName);
            loadedConfigFile = Adapter.ToObject<ConfigFile>(textContent);
        }

        private string GetDefaultConfigFileName()
        {
            var assembly = Assembly.GetEntryAssembly() ?? GetType().Assembly;
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(assembly.Location);
            return $"{nameWithoutExtension}.json";
        }
    }
}