using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(SocialNetworkDbContext context) : base(context)
        {
        }

        public override async Task<bool> DeleteByIdAsync(string id)
        {
            await Context.Messages.Where(m => m.ReplyToId.ToString() == id).LoadAsync();
            return await base.DeleteByIdAsync(id);
        }
    }
}