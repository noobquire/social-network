using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Authorization.Requirements;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class AdminAuthorizationHandler : AuthorizationHandler<SameUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
        {
            if (requirement.AllowAdmin && context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
