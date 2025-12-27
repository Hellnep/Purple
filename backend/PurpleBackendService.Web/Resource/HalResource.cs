namespace PurpleBackendService.Web.Resource
{
    public class HalResource
    {
        public IDictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        public void AddLink(string rel, string href, string method = "GET")
        {
            Links.Add(rel, new Link(href, method));
        }
    }

    public record Link(string Href, string Method);
}