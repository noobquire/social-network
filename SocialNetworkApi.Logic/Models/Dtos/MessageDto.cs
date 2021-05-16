using System;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class MessageDto
    {
        public string Id { get; set; }

        public string AuthorId { get; set; }

        public string Text { get; set; }

        public DateTime TimePublished { get; set; }

        public string ReplyToId { get; set; }
    }
}
