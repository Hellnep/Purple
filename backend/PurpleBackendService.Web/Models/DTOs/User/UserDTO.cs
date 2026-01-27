using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PurpleBackendService.Core.DTOs.User
{
    public class UserDTO
    {
        public long Id { get; set; }

        [MinLength(4), MaxLength(40)]
        public string? Nickname { get; set; }

        public DateTime? JoinedAt { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Phone { get; set;}
    }
}
