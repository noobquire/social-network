using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetworkApi.Data.Repositories
{
    public sealed class ImageRepository : Repository<Image>
    {
        public ImageRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public override async Task<bool> DeleteByIdAsync(string id)
        {
            await Context.Profiles.Where(p => p.AvatarId.ToString() == id).LoadAsync();
            await Context.Posts.Where(p => p.AttachedImageId.ToString() == id).LoadAsync();
            return await base.DeleteByIdAsync(id);
        }
    }
}