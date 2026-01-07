using Microsoft.AspNetCore.Http;
using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IImageService
    {
        public Task<OperationResult<ICollection<ImageDTO>>> AddImagesAsync(long id, IFormFileCollection files);

        public OperationResult<ImageDTO> GetImage(long id);

        public OperationResult<(byte[] content, string contentType)> GetImageFile(long id);

        public OperationResult<(byte[] content, string contentType)> GetImageFile(string fileName);

        public Task<OperationResult<ImageDTO>> ChangeImageAsync(long id, IFormFile file);
    }
}