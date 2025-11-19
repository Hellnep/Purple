using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.DTO.Sql;

public class ProductDTO
{
    public long ProductId { get; set; }

    [MinLength(4), MaxLength(40)]
    public string? Name { get; set; }

    [Required]
    [JsonIgnore]
    public CustomerDTO? Author { get; set; }

    public DateOnly? Date { get; set; }    

    [MaxLength(200)]
    public string? Description { get; set; }
} 