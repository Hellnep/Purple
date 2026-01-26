using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PurpleBackendService.Domain.DTO
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
