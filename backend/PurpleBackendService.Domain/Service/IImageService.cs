using Microsoft.AspNetCore.Http;
using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IImageService
    {
        public Task<OperationResult<ICollection<ProductDTO>>> AddImagesAsync(long id, IFormFileCollection files);

        public Task<OperationResult<ImageDTO>> GetImageAsync(long id);

        public Task<OperationResult<ImageDTO>> ChangeImageAsync(long id, IFormFile file);
    }
}