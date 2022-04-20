using System;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class PostDto
    {
        public string Id { get; set; }

        public string AuthorId { get; set; }

        public string ProfileId { get; set; }

        public string AttachedImageId { get; set; }

        public string Text { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
