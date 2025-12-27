using Newtonsoft.Json;

namespace PurpleBackendService.Web.Resource
{
    public class Resource<T> : HalResource
    {
        public T Data { get; set; }

        public Resource(T data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}