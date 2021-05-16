using System.Diagnostics.CodeAnalysis;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class MessageExtensions
    {
        public static MessageDto ToDto([NotNull] this Message message)
        {
            return new MessageDto
            {
                Id = message.Id.ToString(),
                AuthorId = message.AuthorId.ToString(),
                ReplyToId = message.ReplyTo.Id.ToString(),
                Text = message.Text,
                TimePublished = message.TimePublished
            };
        }
    }
}
