using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public sealed class ImageRepository : Repository<Image>
    {
        public ImageRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}