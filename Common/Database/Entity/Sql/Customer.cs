using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Purple.Common.Database.Entity.Sql;

[Table("Customers")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CustomerId { get; set; }

    [Required]
    [MinLength(4), StringLength(40)]
    [Column("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [Column("RegistrationDate")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [EmailAddress]
    [Column("EmailAddress", TypeName = "varchar(60)")]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set;}

    public ICollection<Product> Products { get; set;}
}