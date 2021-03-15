using System;
using System.Collections.Generic;

namespace SocialNetworkApi.Data.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public List<Post> Posts { get; set; }
        public Image Avatar { get; set; }
    }
}