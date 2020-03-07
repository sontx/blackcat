using Blackcat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Xml;
using System.Xml.Linq;

namespace Blackcat.Configuration
{
    public class XmlDataAdapter : IDataAdapter
    {
        private readonly IContractResolver _contractResolver = new CamelCaseNamingContractResolver();
        private readonly IDataAdapter _jsonAdapter = new JsonDataAdapter();

        public T ToObject<T>(object obj) where T : class
        {
            return ToObject(obj, typeof(T)) as T;
        }

        public object ToObject(object obj, Type convertTo)
        {
            if (!(obj is string xml))
                return _jsonAdapter.ToObject(obj, convertTo);

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var json = JsonConvert.SerializeXmlNode(doc);
            var jObj = _jsonAdapter.ToObject<JObject>(json);
            return _jsonAdapter.ToObject(jObj.Value<JObject>("root").ToString(), convertTo);
        }

        public string ToString(object data)
        {
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = _contractResolver,
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
            var doc = JsonConvert.DeserializeXmlNode(json, "root");
            return XDocument.Parse(doc.OuterXml).ToString();
        }
    }
}