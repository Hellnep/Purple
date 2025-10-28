using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.Entity.Sqlite;

public class Customer
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(20)]
    public required string Username { get; set; }

    public DateOnly? Date { get; set; }
}