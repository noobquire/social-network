using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public sealed class ChatRepository : Repository<Chat>
    {
        public ChatRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}
