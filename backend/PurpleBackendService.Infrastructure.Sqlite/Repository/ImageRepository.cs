using Microsoft.EntityFrameworkCore;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Interfaces.Repositories;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Infrastucture.Repository
{
    public class ImageRepository(PurpleOcean repository)
        : Repository(repository), IImageRepository
    {
        ///<summary>
        /// Add image data to database
        /// </summary>
        /// <param name="image">Image data</param>
        /// <returns>Image entity</returns>
        public Task<Image> Add(Image image)
        {
            var productRefId = image.ProductRefId;
            var product = _repository.Products
                .Include(product => product.Images)
                .FirstOrDefault(product => product.Id == productRefId);

            if (product is null)
            {
                throw new ArgumentNullException($"Product with Id={productRefId} not found");
            }

            product.Images ??= [];
            product.Images.Add(image);

            return _repository
                .SaveChangesAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return image;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to save changes");
                    }
                });
        }

        ///<summary>
        /// Get an image by ID
        /// </summary>
        /// <param name="imageId">Image identification</param>
        /// <returns>Image entity</returns>
        public Task<Image?> Get(long imageId) => _repository.Images
            .Include(image => image.Product)
            .FirstOrDefaultAsync(image => image.Id == imageId);

        ///<summary>
        /// Get an image by path
        /// </summary>
        /// <param name="path">Image path</param>
        /// <returns>Image entity</returns>
        public Task<Image?> Get(string path) => _repository.Images
            .Include(image => image.Product)
            .FirstOrDefaultAsync(image => image.Path == path);

        ///<summary>
        /// Update datas in database
        /// </summary>
        public Task<int> Update() => _repository.SaveChangesAsync();
    }
}
