using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBackendService.Domain.Entity;

[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProductId { get; set; }

    [Column("DateOfCreate")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [ForeignKey("Author")]
    public long? AuthorRefId { get; set; }

    public Customer? Author { get; set; }

    [Required]
    [MinLength(3), StringLength(40)]
    [Column("Title")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("ProductDescription")]
    public string? Description { get; set; } = null!;
}
