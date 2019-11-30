using Blackcat.Utils;
using Newtonsoft.Json;
using System;

namespace Blackcat.Intercomm
{
    internal sealed class JsonContentAdapter : IContentAdapter
    {
        private readonly JsonSerializerSettings jsonSettings;

        public JsonContentAdapter()
        {
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCaseNamingContractResolver() };
        }

        public T ToObject<T>(string st)
        {
            return JsonConvert.DeserializeObject<T>(st, jsonSettings);
        }

        public object ToObject(string st, Type objType)
        {
            return JsonConvert.DeserializeObject(st, objType, jsonSettings);
        }

        public string ToString(object data)
        {
            return JsonConvert.SerializeObject(data, jsonSettings);
        }
    }
}