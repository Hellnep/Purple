using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.DTO.Sql;

public class ProductDTO
{
    public long? Id { get; set; }

    [MinLength(4), MaxLength(40)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}