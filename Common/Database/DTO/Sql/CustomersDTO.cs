using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.DTO.Sql;

public class CustomerDTO
{
    public long CustomerId { get; set; }
    
    [MinLength(4), MaxLength(40)]
    public string? FirstName { get; set; }
    
    public DateOnly? Date { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set;}

    public List<ProductDTO>? Products { get; set;}
}