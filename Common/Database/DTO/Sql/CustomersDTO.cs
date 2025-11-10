using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.DTO.Sql;

public class CustomerDTO
{
    public long? Id { get; set; }
    
    [MinLength(4), MaxLength(40)]
    public string? FirstName { get; set; }
    
    public DateOnly? Date { get; init; }

    [EmailAddress]
    public string? Email { get; set; }
}