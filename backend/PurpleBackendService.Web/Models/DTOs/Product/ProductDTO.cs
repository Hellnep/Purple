using System.ComponentModel.DataAnnotations;

using PurpleBackendService.Web.Models.DTOs.User;
using PurpleBackendService.Web.Models.DTOs.Image;

namespace PurpleBackendService.Web.Models.DTOs.Product
{
    public class ProductDTO
    {
        public long Id { get; set; }

        [MinLength(4), MaxLength(40)]
        public string? Title { get; set; }

        public UserDTO? Author { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        [MaxLength(200)]
        public string? Content { get; set; }

        public ICollection<ImageDTO>? Images { get; set; }
    }
}
