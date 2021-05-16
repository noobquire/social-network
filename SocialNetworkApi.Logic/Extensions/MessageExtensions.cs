using System;
using System.Diagnostics.CodeAnalysis;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
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
                ReplyToId = message.ReplyTo?.Id.ToString(),
                Text = message.Text,
                TimePublished = message.TimePublished,
                ChatId = message.ChatId.ToString()
            };
        }

        public static void Update(this Message message, MessageDataModel messageData)
        {
            message.ReplyToId = messageData.ReplyToId == null ? null : new Guid?(new Guid(messageData.ReplyToId));
            message.Text = messageData.Text;
        }
    }
}
