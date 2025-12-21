using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using PurpleBackendService.Core.Utility;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Domain.Service;

namespace PurpleBackendService.Core.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repository;
        private readonly IHostEnvironment _environment;
        private readonly string _imageStoragePath;

        public ImageService(IImageRepository repository, IHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
            _imageStoragePath = Path.Combine(_environment.ContentRootPath, "uploads");

            // Checking if the folder exists
            // Проверяем, существует ли папка
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }
        }

        public async Task<OperationResult<ICollection<ImageDTO>>> AddImagesAsync(long productId, IFormFileCollection files)
        {
            var imageDTOs = new List<ImageDTO>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var image = await SaveImageAsync(file, productId);
                    imageDTOs.Add(Mapping.Get<ImageDTO, Domain.Entity.Image>(image));
                }
            }

            return OperationResult<ICollection<ImageDTO>>.Success(imageDTOs);
        }

        public async Task<OperationResult<ImageDTO>> ChangeImageAsync(long imageId, IFormFile file)
        {
            var existingImage = _repository.Get(imageId);
            var oldFilePath = Path.Combine(_imageStoragePath, existingImage.Path);

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            var updatedImage = await SaveImageAsync(file, existingImage.ProductRefId ?? 0);

            existingImage.Title = updatedImage.Title;
            existingImage.Path = updatedImage.Path;
            existingImage.Url = updatedImage.Url;
            existingImage.Width = updatedImage.Width;
            existingImage.Height = updatedImage.Height;
            existingImage.Path = updatedImage.Path;
            existingImage.Updated = DateTime.Now;

            await _repository.Update();

            return OperationResult<ImageDTO>
                .Success(Mapping.Get<ImageDTO, Domain.Entity.Image>(existingImage));
        }

        public async Task<OperationResult<ImageDTO>> GetImageAsync(long imageId)
        {
            var image = _repository.Get(imageId);

            if (image is null)
            {
                return OperationResult<ImageDTO>.Failure("Image not found");
            }

            return OperationResult<ImageDTO>
                .Success(Mapping.Get<ImageDTO, Domain.Entity.Image>(image));
        }

        // Processing the image using ImageSharp
        // Обрабатываем изображение с помощью ImageSharp
        private async Task<Domain.Entity.Image> SaveImageAsync(IFormFile file, long productId)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_imageStoragePath, fileName);

            using (var stream = file.OpenReadStream())
            {
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(stream))
                {
                    await image.SaveAsync(filePath);

                    // Creating an image entity
                    var imageEntity = new Domain.Entity.Image
                    {
                        Title = file.FileName,
                        Path = fileName,
                        Url = $"/images{fileName}",
                        Length = file.Length,
                        Width = image.Width,
                        Height = image.Height,
                        ProductRefId = productId
                    };

                    return await _repository.Add(imageEntity);
                } 
            }
        }
    }
}