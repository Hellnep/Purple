using Microsoft.AspNetCore.Http;

using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Core.Interfaces.Services
{
    public interface IImageService
    {
        public Task<OperationResult<ICollection<Image>>> AddImagesAsync(long id, IFormFileCollection files);

        public Task<OperationResult<Image>> GetImageAsync(long id);

        public Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(long id);

        public Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(string fileName);

        public Task<OperationResult<Image>> ChangeImageAsync(long id, IFormFile file);
    }
}
