using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Purple.Common.Database.Entity.Sql;

[Table("Products")]
public class Product
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MinLength(5), StringLength(40)]
    [Display(Name = "Title")]
    public required string Name { get; set; }

    [MaxLength(200)]
    [Column("ProductDescription")]
    public string? Description { get; set; } = null!;
}
