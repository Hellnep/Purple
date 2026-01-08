using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface IImageRepository
    {
        public Task<Image?> Add(Image image);

        public Task<Image?> Get(long imageId);

        public Task<Image?> Get(string path);

        public Task<int> Update();
    }
}