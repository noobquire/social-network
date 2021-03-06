using SocialNetworkApi.Data.Models;
using System.Threading.Tasks;

namespace SocialNetworkApi.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Post> Posts { get; }
        IRepository<Chat> Chats { get; }
        IRepository<Image> Images { get; }
        IRepository<Message> Messages { get; }
        IRepository<Profile> Profiles { get; }
        IRepository<User> Users { get; }

        Task SaveChangesAsync();
    }
}