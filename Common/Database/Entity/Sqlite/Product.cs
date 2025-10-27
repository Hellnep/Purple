using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.Entity.Sqlite;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(25)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? Description { get; set; } = null!;
}
