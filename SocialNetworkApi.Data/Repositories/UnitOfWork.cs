using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using System.Threading.Tasks;

namespace SocialNetworkApi.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SocialNetworkDbContext _context;
        public IRepository<Post> Posts { get; }
        public IRepository<Chat> Chats { get; }
        public IRepository<Image> Images { get; }
        public IRepository<Message> Messages { get; }
        public IRepository<Profile> Profiles { get; }
        public IRepository<User> Users { get; }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public UnitOfWork(IRepository<Post> posts, IRepository<Chat> chats, IRepository<Image> images, IRepository<Message> messages, IRepository<Profile> profiles, IRepository<User> users, SocialNetworkDbContext context)
        {
            Posts = posts;
            Chats = chats;
            Images = images;
            Messages = messages;
            Profiles = profiles;
            Users = users;
            _context = context;
        }
    }
}