using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Purple.Common.Database.Entity.Sql;

[Table("Customers")]
public class Customer
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MinLength(5), StringLength(32)]
    [Column("Name")]
    public required string Username { get; set; }

    [Column("RegistrationDate")]
    public DateOnly Date { get; set; }
}