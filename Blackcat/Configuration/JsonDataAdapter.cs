using Blackcat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Blackcat.Configuration
{
    public class JsonDataAdapter : IDataAdapter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly JsonSerializer _jsonSerializer;

        public JsonDataAdapter()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseNamingContractResolver(),
                Formatting = Formatting.Indented
            };
            _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
        }

        public T ToObject<T>(object obj) where T : class
        {
            return ToObject(obj, typeof(T)) as T;
        }

        public object ToObject(object obj, Type convertTo)
        {
            switch (obj)
            {
                case string json:
                    return JsonConvert.DeserializeObject(json, convertTo, _jsonSerializerSettings);

                case null:
                    return null;

                case JObject jObj:
                    return jObj.ToObject(convertTo, _jsonSerializer);
            }

            if (convertTo.IsInstanceOfType(obj))
                return obj;

            throw new ConfigurationIOException($"Can not convert {obj.GetType().Name} to {convertTo.Name}");
        }

        public string ToString(object data)
        {
            return JsonConvert.SerializeObject(data, _jsonSerializerSettings);
        }
    }
}