using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Purple.Common.Database.Entity;

public class Customer
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("[w]+")]
    public required string Username { get; set; }

    [Required]
    public DateOnly Date { get; set; }
}