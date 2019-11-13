using Blackcat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Blackcat.Configuration
{
    public class JsonDataAdapter : IDataAdapter
    {
        private readonly IContractResolver contractResolver = new CamelCaseNamingContractResolver();

        public T ToObject<T>(object obj) where T : class
        {
            if (obj is string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }

            if (obj == null)
                return null;

            if (obj is JObject jObj)
                return jObj.ToObject<T>(new JsonSerializer { ContractResolver = contractResolver });

            if (obj is T)
                return (T)obj;

            throw new ConfigurationIOException($"Can not convert {obj.GetType().Name} to {typeof(T).Name}");
        }

        public string ToString(object data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }
    }
}