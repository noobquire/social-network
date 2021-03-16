using System;
using System.Collections.Generic;
using System.Text;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public sealed class PostRepository : Repository<Post>
    {
        public PostRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}
