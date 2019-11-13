using Blackcat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Blackcat.Configuration
{
    public class XmlDataAdapter : IDataAdapter
    {
        private readonly IContractResolver contractResolver = new CamelCaseNamingContractResolver();
        private readonly IDataAdapter jsonAdapter = new JsonDataAdapter();

        public T ToObject<T>(object obj) where T : class
        {
            if (obj is string xml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var json = JsonConvert.SerializeXmlNode(doc);
                var jObj = jsonAdapter.ToObject<JObject>(json);
                return jsonAdapter.ToObject<T>(jObj.Value<JObject>("root").ToString());
            }

            return jsonAdapter.ToObject<T>(obj);
        }

        public string ToString(object data)
        {
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
            var doc = JsonConvert.DeserializeXmlNode(json, "root");
            return XDocument.Parse(doc.OuterXml).ToString();
        }
    }
}