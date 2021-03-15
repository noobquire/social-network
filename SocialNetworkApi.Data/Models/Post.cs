using System;

namespace SocialNetworkApi.Data.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public User Author { get; set; }
        public Profile Profile { get; set; }
        public Image AttachedImage { get; set; }
        public string Text { get; set; }
        public DateTime TimePublished { get; set; }
    }
}
