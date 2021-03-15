using System;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a many-to-many join table between users and chats.
    /// </summary>
    public class UserChat
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Chat Chat { get; set; }
        public Guid ChatId { get; set; }
    }
}
