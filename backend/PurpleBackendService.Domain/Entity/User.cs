using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBackendService.Domain.Entity
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MinLength(4), StringLength(40)]
        [Column("FirstName")]
        public string Nickname { get; set; } = string.Empty;

        [Column("RegistrationDate")]
        public DateTime JoinedAt { get; set; } = DateTime.Now;

        [EmailAddress]
        [Column("EmailAddress", TypeName = "varchar(60)")]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set;}

        public ICollection<Product>? Products { get; set;}
    }
}