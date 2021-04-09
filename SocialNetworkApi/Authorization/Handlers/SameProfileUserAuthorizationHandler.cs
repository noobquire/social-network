using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class SameProfileUserAuthorizationHandler :
        AuthorizationHandler<SameUserRequirement, ProfileDto>
    {
        private readonly UserManager<User> _userManager;

        public SameProfileUserAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, ProfileDto resource)
        {
            var authUser = await _userManager.GetUserAsync(context.User);
            if (authUser.Id.ToString() == resource.UserId)
            {
                context.Succeed(requirement);
            }
        }
    }
}