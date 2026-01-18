using System.ComponentModel.DataAnnotations;

namespace PurpleBackendService.Domain.DTO
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
        public string? Phone { get; set;}
    }
}