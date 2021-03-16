﻿using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<Post> Posts { get; }
        public IRepository<Chat> Chats { get; }
        public IRepository<Image> Images { get; }
        public IRepository<Message> Messages { get; }
        public IRepository<Profile> Profiles { get; }
        public IRepository<User> Users { get; }

        public UnitOfWork(IRepository<Post> posts, IRepository<Chat> chats, IRepository<Image> images, IRepository<Message> messages, IRepository<Profile> profiles, IRepository<User> users)
        {
            Posts = posts;
            Chats = chats;
            Images = images;
            Messages = messages;
            Profiles = profiles;
            Users = users;
        }
    }
}