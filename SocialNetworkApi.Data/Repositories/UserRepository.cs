using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}