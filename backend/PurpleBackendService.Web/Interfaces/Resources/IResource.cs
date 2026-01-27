using PurpleBackendService.Core.Common;
using PurpleBackendService.Web.Models.Resources;

namespace PurpleBackendService.Web.Interfaces.Resources
{
    public interface IResource
    {
        public IDictionary<string, LinkDTO> Links { get; }

        public void AddLink(string name, string href, string method = "GET");
    }
}
