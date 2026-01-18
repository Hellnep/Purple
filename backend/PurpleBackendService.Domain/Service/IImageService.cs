using Microsoft.AspNetCore.Http;
using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IImageService
    {
        public Task<OperationResult<ICollection<ImageDTO>>> AddImagesAsync(long id, IFormFileCollection files);

        public Task<OperationResult<ImageDTO>> GetImageAsync(long id);

        public Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(long id);

        public Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(string fileName);

        public Task<OperationResult<ImageDTO>> ChangeImageAsync(long id, IFormFile file);
    }
}