using SixLabors.ImageSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using PurpleBackendService.Infrastucture.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Core.DTOs.Image;
using PurpleBackendService.Domain.Interfaces.Repositories;

namespace PurpleBackendService.Core.Services
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
            _imageStoragePath = Path.Combine(_environment.ContentRootPath, "images");

            // Checking if the folder exists
            // Проверяем, существует ли папка
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }
        }

        ///<summary>
        /// Adding images to the database
        /// </summary>
        /// <param name="productId">Identification the product</param>
        /// <param name="files">Image files</param>
        /// <returns>Returns a collection of image metadata</returns>
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

        ///<summary>
        /// Changing the image
        /// </summary>
        /// <param name="imageId">Identification the image</param>
        /// <param name="file">Image file</param>
        /// <returns>Returns an image metadata</returns>
        public async Task<OperationResult<ImageDTO>> ChangeImageAsync(long imageId, IFormFile file)
        {
            var existingImage = await _repository.Get(imageId);
            var oldFilePath = Path.Combine(_imageStoragePath, existingImage!.Path);

            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }

            var updatedImage = await SaveImageAsync(file, existingImage.ProductRefId ?? 0);

            existingImage.Title = updatedImage.Title;
            existingImage.Path = updatedImage.Path;
            existingImage.RelativePath = updatedImage.RelativePath;
            existingImage.Width = updatedImage.Width;
            existingImage.Height = updatedImage.Height;
            existingImage.Path = updatedImage.Path;
            existingImage.Updated = DateTime.Now;

            await _repository.Update();

            return OperationResult<ImageDTO>
                .Success(Mapping.Get<ImageDTO, Domain.Entity.Image>(existingImage));
        }

        ///<summary>
        /// Provides metadata for an image from database
        /// </summary>
        /// <param name="imageId">Identification the image</param>
        /// <returns>Returns an image metadata</returns>
        public async Task<OperationResult<ImageDTO>> GetImageAsync(long imageId)
        {
            var image = await _repository.Get(imageId);

            if (image is null)
            {
                return OperationResult<ImageDTO>.Failure("Image not found");
            }

            return OperationResult<ImageDTO>
                .Success(Mapping.Get<ImageDTO, Domain.Entity.Image>(image));
        }

        /// <summary>
        /// Provides a file with a bit format for output as an image
        /// </summary>
        /// <param name="imageId">Identification the image</param>
        /// <returns>Returns an array of image bits</returns>
        public async Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(long imageId)
        {
            var image = await _repository.Get(imageId);

            if (image is null)
            {
                return OperationResult<(byte[], string)>.Failure("Image not found");
            }

            var filePath = Path.Combine(_imageStoragePath, image.Path);

            if (!File.Exists(filePath))
            {
                return OperationResult<(byte[], string)>.Failure("Image file not found");
            }

            try
            {
                var content = File.ReadAllBytes(filePath);
                var contentType = GetContentType(image.Path);

                return OperationResult<(byte[], string)>.Success((content, contentType));
            }
            catch (Exception ex)
            {
                return OperationResult<(byte[], string)>.Failure($"Error reading image file: {ex.Message}");
            }
        }

        ///<summary>
        /// Provides a file with a bit format for output as an image
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>Returns an array of image bits</returns>
        public async Task<OperationResult<(byte[] content, string contentType)>> GetImageFileAsync(string fileName)
        {
            // Getting the image metadata in database
            // Получение метаданных изображения из базы данных
            var image = await _repository.Get(fileName);

            if (image is null)
            {
                return OperationResult<(byte[], string)>.Failure("Image not found by file name");
            }

            // Getting the file path
            // Получение пути к файлу
            var filePath = Path.Combine(_imageStoragePath, image.Path);

            if (!File.Exists(filePath))
            {
                return OperationResult<(byte[], string)>.Failure("Image file not found in storage");
            }

            // Reading the image from file
            // Читаем изображение из файла
            try
            {
                var content = File.ReadAllBytes(filePath);
                var contentType = GetContentType(image.Path);

                return OperationResult<(byte[], string)>.Success((content, contentType));
            }
            catch (Exception ex)
            {
                return OperationResult<(byte[], string)>.Failure($"Error reading image file: {ex.Message}");
            }
        }

        /// <summary>
        /// Saving an image in a local directory and in a database
        /// </summary>
        /// <param name="file">The image to save</param>
        /// <param name="productId">Identification the product</param>
        /// <returns>Return saved image in database</returns>
        private async Task<Domain.Entity.Image> SaveImageAsync(IFormFile file, long productId)
        {
            // Processing the image using ImageSharp
            // Обрабатываем изображение с помощью ImageSharp
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_imageStoragePath, fileName);

            using (var stream = file.OpenReadStream())
            {
                using (var image = await Image.LoadAsync(stream))
                {
                    await image.SaveAsync(filePath);

                    // Creating an image entity
                    var imageEntity = new Domain.Entity.Image
                    {
                        Title = file.FileName,
                        Path = fileName,
                        RelativePath = $"/api/images/{fileName}",
                        Length = file.Length,
                        Width = image.Width,
                        Height = image.Height,
                        ProductRefId = productId
                    };

                    return await _repository.Add(imageEntity);
                }
            }
        }

        /// <summary>
        /// Defines the image file extension
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>Returns the content type for the responce</returns>
        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
