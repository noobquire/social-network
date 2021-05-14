using System;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a many-to-many join table between users and chats.
    /// </summary>
    public class UserChat
    {
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
        public virtual Chat Chat { get; set; }
        public Guid ChatId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
