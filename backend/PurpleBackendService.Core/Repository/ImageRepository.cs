using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
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

        public async Task<Image?> Add(Image image)
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

            await Update();
            return image;
        }

        public async Task<Image?> Get(long imageId)
        {
            return await _repository.Images
                .Include(image => image.Product)
                .FirstOrDefaultAsync(image => image.Id == imageId);
        }

        public async Task<Image?> Get(string path)
        {
            return await _repository.Images
                .Include(image => image.Product)
                .FirstOrDefaultAsync(image => image.Path == path);
        }

        public Task<int> Update() =>
            _repository.SaveChangesAsync();
    }
}