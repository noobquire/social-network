using System;
using System.Diagnostics.CodeAnalysis;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class ProfileExtensions
    {
        public static ProfileDto ToDto([NotNull] this Profile profile)
        {
            return new ProfileDto()
            {
                Id = profile.Id.ToString(),
                AvatarId = profile.AvatarId?.ToString(),
                Status = profile.Status,
                UserId = profile.UserId.ToString()
            };
        }

        public static void Update([NotNull] this Profile profile, [NotNull] ProfileDto dto)
        {
            profile.Status = dto.Status ?? profile.Status;
            profile.AvatarId = dto.AvatarId == null ? profile.AvatarId : new Guid(dto.AvatarId);
        }
    }
}
