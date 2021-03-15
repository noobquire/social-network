using System;

namespace SocialNetworkApi.Data.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public User Author { get; set; }
        public string Text { get; set; }
        public string TimePublished { get; set; }
        public Chat Chat { get; set; }
        public Message ReplyTo { get; set; }
    }
}
