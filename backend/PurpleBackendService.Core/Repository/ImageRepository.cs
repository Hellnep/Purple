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

        public async Task<Image> Add(Image image)
        {
            if (image.ProductRefId is null)
            {
                throw new ArgumentNullException("The product Id was not transmitted");
            }
            
            var productRefId = image.ProductRefId;
            var product = _repository.Products
                .Include(product => product.Images)
                .FirstOrDefault(product => product.Id == productRefId);
            
            if (product is null)
            {
                throw new ArgumentNullException($"Product with Id={productRefId} not found");
            }

            product.Images ??= new List<Image>();
            product.Images.Add(image);

            await _repository.SaveChangesAsync();
            return image;
        }

        public Image Get(long imageId)
        {
            var image = _repository.Images
                .Include(image => image.Product)
                .FirstOrDefault(image => image.ProductRefId == imageId);

            if (image is null)
            {
                throw new ArgumentNullException("The returned DbContext object has a null value");
            }

            return image;
        }

        public async Task<int> Update() =>
            await _repository.SaveChangesAsync();
    }
}