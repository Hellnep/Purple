using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class ImageRepository : Repository, IImageRepository
    {
        public ImageRepository(PurpleOcean repository) : base(repository)
        {
        }

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

        public Task<Image?> Get(long imageId) =>
            _repository.Images
                .Include(image => image.Product)
                .FirstOrDefaultAsync(image => image.Id == imageId);

        public Task<Image?> Get(string path) =>
            _repository.Images
                .Include(image => image.Product)
                .FirstOrDefaultAsync(image => image.Path == path);

        public Task<int> Update() =>
            _repository.SaveChangesAsync();
    }
}