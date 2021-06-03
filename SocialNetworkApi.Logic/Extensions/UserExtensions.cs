﻿using System.Diagnostics.CodeAnalysis;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto([NotNull] this User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id.ToString(),
                ProfileId = user.Profile?.Id.ToString(),
                IsDeleted = user.IsDeleted
            };
        }
    }
}
