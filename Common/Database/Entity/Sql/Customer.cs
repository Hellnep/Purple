using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Purple.Common.Database.Entity.Sql;

[Table("Customers")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MinLength(5), StringLength(32)]
    [Column("Name")]
    public string Username { get; set; } = string.Empty;

    [Column("RegistrationDate")]
    public DateOnly Date { get; set; }
}