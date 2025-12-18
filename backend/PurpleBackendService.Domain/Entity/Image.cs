using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBackendService.Domain.Entity
{
    [Table("Images")]
    public class Images
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("FileName")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Path { get; set; } = string.Empty;
        
        public string Url { get; set; } = string.Empty;

        public long Weight { get; set; }

        public long Width { get; set; }

        public long Height { get; set; }

        [Column("Created_at")]
        public DateOnly Created { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Column("Updated_at")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}