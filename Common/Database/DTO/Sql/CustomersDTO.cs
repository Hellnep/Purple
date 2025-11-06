namespace Purple.Common.Database.DTO.Sql;

public class CustomerDTO
{
    public long? Id { get; set; }
    public string? FirstName { get; set; }
    public DateOnly Date { get; set; }
    public string? Email { get; set; }
}