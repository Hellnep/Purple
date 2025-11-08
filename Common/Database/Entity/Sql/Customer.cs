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
    [Column("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [Column("RegistrationDate")]
    public DateOnly Date { get; set; }

    [EmailAddress]
    [Column("EmailAddress", TypeName = "varchar(60)")]
    public string? Email { get; set; }
}