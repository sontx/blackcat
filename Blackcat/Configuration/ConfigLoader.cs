using Blackcat.Configuration.AutoNotifyPropertyChange;
using Blackcat.Internal;
using Blackcat.Types;
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

        private ConcurrentDictionary<string, object> _configDict = new ConcurrentDictionary<string, object>();
        private readonly object _lockLoadIndividualConfig = new object();

        private ConfigFile _loadedConfigFile;
        private SaveMode _saveMode;
        private bool _disposed;
        private IDataStorage _storage;
        private IApplicationExitDispatcher _applicationExitDispatcher;

        public SaveMode SaveMode
        {
            get => _saveMode;
            set
            {
                if (_saveMode == value) return;

                if (value == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(_configDict);
                    foreach (var data in dict.Values)
                    {
                        SubscribeChanges(data);
                    }
                }
                else if (value == SaveMode.ReadOnly && _saveMode == SaveMode.OnChange)
                {
                    var dict = new Dictionary<string, object>(_configDict);
                    foreach (var data in dict.Values)
                    {
                        UnsubscribeChanges(data);
                    }
                }
                _saveMode = value;
            }
        }

        public IDataAdapter Adapter { get; set; }

        public IDataStorage Storage
        {
            get => _storage;
            set
            {
                Precondition.PropertyNotNull(value, nameof(Storage));
                _storage = value;
                ReloadConfig();
            }
        }

        public IApplicationExitDispatcher ApplicationExitDispatcher
        {
            get => _applicationExitDispatcher;
            set
            {
                if (_applicationExitDispatcher != null)
                    _applicationExitDispatcher.Exit -= Application_ApplicationExit;

                _applicationExitDispatcher = value;

                if (_applicationExitDispatcher != null)
                    _applicationExitDispatcher.Exit += Application_ApplicationExit;
            }
        }

        public ConfigLoader()
        {
            ApplicationExitDispatcher = new ApplicationExitDispatcher();
            Adapter = new JsonDataAdapter();
            Storage = new FileDataStorage();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (SaveMode != SaveMode.ReadOnly)
            {
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            SaveSpecificConfig(_configDict, true);
        }

        private ConfigFile SaveSpecificConfig(IDictionary<string, object> srcDict, bool overwrite)
        {
            if (SaveMode == SaveMode.ReadOnly)
                return null;

            var configFile = CreateConfigFile(srcDict);
            var contentToSave = Adapter.ToString(configFile);
            Storage.Save(contentToSave, overwrite);
            return configFile;
        }

        private ConfigFile CreateConfigFile(IDictionary<string, object> srcDict)
        {
            var dict = new Dictionary<string, object>(srcDict);
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
            return configFile;
        }

        public void InitializeSettings(object[] defaultSettings)
        {
            Precondition.ArgumentNotNull(defaultSettings, nameof(defaultSettings));

            var cloned = defaultSettings.Clone() as object[];
            Array.Reverse(cloned);
            if (Storage.IsPresented)
                VerifySettings(cloned);
            else
                LoadDefaultSettings(cloned);
        }

        private void LoadDefaultSettings(IReadOnlyCollection<object> cloned)
        {
            var dict = new Dictionary<string, object>(cloned.Count);
            foreach (var setting in cloned)
            {
                var configKey = GetConfigKey(setting.GetType());
                dict.Add(configKey, setting);
            }

            if (_saveMode == SaveMode.ReadOnly)
            {
                _configDict = new ConcurrentDictionary<string, object>(dict);
            }
            else
            {
                var savedConfigFile = SaveSpecificConfig(dict, false);
                ReloadConfig(savedConfigFile);
            }
        }

        private void VerifySettings(IEnumerable<object> defaultSettings)
        {
            var dict = new Dictionary<string, object>();
            var verifyFailed = false;
            foreach (var setting in defaultSettings)
            {
                var type = setting.GetType();
                var configKey = GetConfigKey(type);
                try
                {
                    if (!dict.ContainsKey(configKey))
                    {
                        var validSetting = Get(type);
                        dict.Add(configKey, validSetting);
                    }
                }
                catch
                {
                    dict.Add(configKey, setting);
                    verifyFailed = true;
                }
            }

            if (!verifyFailed) return;

            if (_saveMode == SaveMode.ReadOnly)
            {
                _configDict = new ConcurrentDictionary<string, object>(dict);
            }
            else
            {
                var savedConfigFile = SaveSpecificConfig(dict, true);
                ReloadConfig(savedConfigFile);
            }
        }

        public T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public object Get(Type configType)
        {
            var requestKey = GetConfigKey(configType);
            lock (_lockLoadIndividualConfig)
            {
                if (!_configDict.ContainsKey(requestKey))
                    CacheOrCreateConfig(configType);
            }

            if (_configDict.TryGetValue(requestKey, out var ret))
                return ret;
            return null;
        }

        private string GetConfigKey(Type requestType)
        {
            var configAttr = requestType.GetCustomAttribute<ConfigClassAttribute>();
            if (configAttr == null || string.IsNullOrEmpty(configAttr.Key))
            {
                return requestType.Name;
            }
            return string.IsNullOrEmpty(configAttr.Key)
                ? requestType.Name
                : configAttr.Key;
        }

        private void CacheOrCreateConfig(Type configType)
        {
            var configKey = GetConfigKey(configType);
            var found = _loadedConfigFile.Configs.Find(config => string.CompareOrdinal(configKey, config.Key) == 0);
            if (found != null)
            {
                var data = Adapter.ToObject(found.Data, configType);
                _configDict.TryAdd(configKey, PreprocessLoadedData(data));
            }
            else
            {
                var newConfigInstance = Activator.CreateInstance(configType);
                _configDict.TryAdd(configKey, PreprocessLoadedData(newConfigInstance));
            }
        }

        private void ReloadConfig(ConfigFile configFile = null)
        {
            _loadedConfigFile = configFile ?? GetConfigFile();
            _configDict.Clear();
        }

        private ConfigFile GetConfigFile()
        {
            var content = Storage.Load();
            return string.IsNullOrEmpty(content)
                ? ConfigFile.Empty
                : Adapter.ToObject<ConfigFile>(content);
        }

        private object PreprocessLoadedData(object loadedData)
        {
            if (!(loadedData is AutoNotifyPropertyChanged)) return loadedData;

            var decoratedData = AutoNotifyPropertyChanged.CreateInstance(loadedData.GetType());
            decoratedData.Populate(loadedData);
            if (SaveMode == SaveMode.OnChange)
            {
                SubscribeChanges(decoratedData);
            }
            return decoratedData;
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
            Task.Run(SaveConfig);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (SaveMode != SaveMode.ReadOnly)
            {
                SaveConfig();
            }

            Storage?.Dispose();
        }
    }
}