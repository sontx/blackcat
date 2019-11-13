using Newtonsoft.Json.Serialization;

namespace Blackcat.Utils
{
    internal class CamelCaseNamingContractResolver : SerializableExpandableContractResolver
    {
        public CamelCaseNamingContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }
    }
}