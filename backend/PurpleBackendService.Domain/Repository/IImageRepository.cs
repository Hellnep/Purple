using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface IImageRepository
    {
        public Task<Image> Add(Image image);

        public Image Get(long imageId);

        public Task<int> Update();
    }
}