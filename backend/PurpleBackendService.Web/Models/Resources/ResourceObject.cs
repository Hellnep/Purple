using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PurpleBackendService.Web.Models.Resources;

namespace PurpleBackendService.Web.Resource
{
    public class ResourceObject<T>(T data) : HalResource where T : class
    {
        [JsonIgnore]
        public T Data { get; set; } = data;

        [JsonExtensionData]
        private IDictionary<string, JToken> _unwrappedData => JObject.FromObject(Data)!;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
