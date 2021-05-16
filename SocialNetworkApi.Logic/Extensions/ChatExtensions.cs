using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class ChatExtensions
    {
        public static ChatDto ToDto([NotNull] this Chat chat)
        {
            return new ChatDto
            {
                Id = chat.Id.ToString(),
                IsDeleted = chat.IsDeleted,
                Name = chat.Name,
                ParticipantIds = chat.Participants
                    .Select(p =>
                        p.UserId.ToString())
                    .ToList(),
                Type = chat.Type.ToString()
            };
        }
    }
}
