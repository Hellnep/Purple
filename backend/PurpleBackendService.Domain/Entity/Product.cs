using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBackendService.Domain.Entity
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("Created_at")]
        public DateOnly Created { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Column("Updated_at")]
        public DateTime Updated { get; set; } = DateTime.Now;

        [ForeignKey("Author")]
        public long? AuthorRefId { get; set; }

        public Customer? Author { get; set; }

        [Required]
        [MinLength(3), StringLength(40)]
        [Column("Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("Content")]
        public string? Content { get; set; } = null!;

        public ICollection<Image>? Images { get; set; }
    }
}