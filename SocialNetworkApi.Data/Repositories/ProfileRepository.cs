using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public sealed class ProfileRepository : Repository<Profile>
    {
        public ProfileRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}