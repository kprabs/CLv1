using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreLib.Application.Common.Utility
{
    public static class ObjectExtension
    {
        public static string SerializeObjectToJson(this Object obj, bool camelCase = true)
        {
            return camelCase ? 
                JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                                {
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                }) : 
                JsonConvert.SerializeObject(obj);
        }
    }
}
