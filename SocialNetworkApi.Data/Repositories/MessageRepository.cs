using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}