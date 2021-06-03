﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class SamePostAuthorAuthorizationHandler : AuthorizationHandler<SameUserRequirement, PostDto>
    {
        private readonly UserManager<User> _userManager;

        public SamePostAuthorAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, PostDto resource)
        {
            var authUser = await _userManager.GetUserAsync(context.User);
            if (authUser.Id.ToString() == resource.AuthorId)
            {
                context.Succeed(requirement);
            }
        }
    }
}