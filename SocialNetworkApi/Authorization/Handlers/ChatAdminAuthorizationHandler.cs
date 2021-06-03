using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class ChatAdminAuthorizationHandler : AuthorizationHandler<SameUserRequirement, ChatDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IChatsService _chatsService;

        public ChatAdminAuthorizationHandler(UserManager<User> userManager, IChatsService chatsService)
        {
            _userManager = userManager;
            _chatsService = chatsService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, ChatDto resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (requirement.AllowAdmin && await _chatsService.IsAdmin(user.Id.ToString(), resource.Id))
            {
                context.Succeed(requirement);
            }
        }
    }
}
