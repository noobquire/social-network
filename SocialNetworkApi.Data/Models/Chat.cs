using System;
using System.Collections.Generic;

namespace SocialNetworkApi.Data.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public List<User> Participants { get; set; }
        public List<Message> Messages { get; set; }
    }
}