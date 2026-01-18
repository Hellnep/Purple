using Newtonsoft.Json;

namespace PurpleBackendService.Web.Resource
{
    public class Resource<T>(T data) : HalResource
    {
        public T Data { get; set; } = data;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}