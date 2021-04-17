using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class SameUserAuthorizationHandler :
        AuthorizationHandler<SameUserRequirement, UserDto>
    {
        private readonly UserManager<User> _userManager;

        public SameUserAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, UserDto resource)
        {
            var authUser = await _userManager.GetUserAsync(context.User);
            if (authUser?.Id.ToString() == resource.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}