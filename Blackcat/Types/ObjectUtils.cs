using Blackcat.Utils;
using Newtonsoft.Json;

namespace Blackcat.Types
{
    public static class ObjectUtils
    {
        public static void Populate(this object destObj, object srcObj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseNamingContractResolver()
            };
            var json = JsonConvert.SerializeObject(srcObj, settings);
            JsonConvert.PopulateObject(json, destObj, settings);
        }
    }
}