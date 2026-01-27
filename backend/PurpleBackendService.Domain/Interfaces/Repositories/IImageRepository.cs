using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Interfaces.Repositories
{
    public interface IImageRepository
    {
        public Task<Image> Add(Image image);

        public Task<Image?> Get(long imageId);

        public Task<Image?> Get(string path);

        public Task<int> Update();
    }
}
