namespace PurpleBackendService.Domain.DTO
{
    public class ImageDTO
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Url { get; set; }

        public DateOnly? Created { get; set; }

        public DateOnly? Updated { get; set; }
    }
}