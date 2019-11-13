using Newtonsoft.Json.Serialization;

namespace Blackcat.Configuration
{
    internal class CamelCaseNamingContractResolver : SerializableExpandableContractResolver
    {
        public CamelCaseNamingContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }
    }
}