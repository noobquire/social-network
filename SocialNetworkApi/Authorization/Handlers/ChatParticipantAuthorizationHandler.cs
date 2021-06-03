using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class ChatParticipantAuthorizationHandler : AuthorizationHandler<ChatParticipantRequirement, ChatDto>
    {
        private readonly UserManager<User> _userManager;

        public ChatParticipantAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ChatParticipantRequirement requirement, ChatDto resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (resource.ParticipantIds.Contains(user.Id.ToString()))
            {
                context.Succeed(requirement);
            }
        }
    }
}
