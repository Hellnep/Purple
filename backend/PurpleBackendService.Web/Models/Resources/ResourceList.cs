using System.Text.Json.Serialization;
using PurpleBackendService.Web.Models.Resources;

namespace PurpleBackendService.Web.Resource
{
    public class ResourceCollection<T> : HalResource where T : class
    {
        [JsonPropertyName("_embedded")]
        public IList<T> Items { get; set; }

        public ResourceCollection(IEnumerable<T> items)
        {
            Items = items.ToList();
        }
    }
}
