using System.Text.Json.Serialization;
using PurpleBackendService.Core.Common;
using PurpleBackendService.Web.Interfaces.Resources;

namespace PurpleBackendService.Web.Models.Resources
{
    public class HalResource : IResource
    {
        [JsonPropertyName("_links")]
        public IDictionary<string, LinkDTO> Links { get; set; } = new Dictionary<string, LinkDTO>();

        public void AddLink(string rel, string href, string method = "GET")
        {
            Links.Add(rel, new LinkDTO(href, method));
        }
    }

    public record Link(string Href, string Method);
}
