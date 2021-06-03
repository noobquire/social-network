using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Authorization.Requirements;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class EnabledUserAuthorizationHandler : AuthorizationHandler<EnabledUserRequirement>
    {
        private readonly UserManager<User> _userManager;

        public EnabledUserAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EnabledUserRequirement requirement)
        {
            var principal = context.User;
            var loggedInUser = await _userManager.GetUserAsync(principal);
            if (!(loggedInUser?.IsDeleted ?? true))
            {
                context.Succeed(requirement);
            }
        }
    }
}
