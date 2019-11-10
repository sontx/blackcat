using Blackcat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public bool IgnoreCase { get; set; } = true;

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
            SaveConfigToFile();
        }

        public void SaveConfigToFile()
        {
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

            var jsonToSave = JsonConvert.SerializeObject(configFile, Formatting.Indented);
            File.WriteAllText(loadedFileName, jsonToSave);
        }

        public T Get<T>() where T : class
        {
            if (loadedConfigFile == null)
            {
                lock (lockLoadConfigFile)
                {
                    Load();
                }
            }

            var requestType = typeof(T);
            var configAttr = requestType.GetCustomAttribute<ConfigClassAttribute>();
            if (configAttr == null) return null;
            var requestKey = configAttr.Key;

            lock (lockLoadIndividualConfig)
            {
                if (!loadedConfigDict.ContainsKey(requestKey))
                    LoadConfig<T>(requestKey);
            }

            if (loadedConfigDict.TryGetValue(requestKey, out var ret))
                return (T)ret;
            return null;
        }

        private void LoadConfig<T>(string requestKey) where T : class
        {
            var found = loadedConfigFile?.Configs.Find(config => string.Compare(requestKey, config.Key, IgnoreCase) == 0);
            if (found != null)
            {
                var jObj = found.Data as JObject;
                var data = jObj.ToObject<T>();
                loadedConfigDict.TryAdd(requestKey, data);
            }
            else
            {
                var newConfigInstance = Activator.CreateInstance<T>();
                loadedConfigDict.TryAdd(requestKey, newConfigInstance);
            }
        }

        public Task LoadAsync(string fileName = null)
        {
            return Task.Run(() =>
            {
                Load(fileName);
            });
        }

        private void Load(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = GetDefaultConfigFileName();

            loadedFileName = fileName;

            if (!File.Exists(fileName))
            {
                loadedConfigFile = new ConfigFile { Configs = new List<ConfigElement>(0) };
                return;
            }

            var jsonSt = File.ReadAllText(fileName);
            loadedConfigFile = JsonConvert.DeserializeObject<ConfigFile>(jsonSt);
        }

        private string GetDefaultConfigFileName()
        {
            var assembly = Assembly.GetEntryAssembly() ?? GetType().Assembly;
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(assembly.Location);
            return $"{nameWithoutExtension}.config.json".ToLower();
        }
    }
}