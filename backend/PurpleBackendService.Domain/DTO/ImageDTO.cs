namespace PurpleBackendService.Domain.DTO
{
    public class ImageDTO
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? RelativePath { get; set; }

        public DateOnly? Created { get; set; }

        public DateTime Updated { get; set; }
    }
}