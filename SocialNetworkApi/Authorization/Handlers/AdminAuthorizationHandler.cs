using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SocialNetworkApi.Authorization.Handlers
{
    public class AdminAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if(context.User.IsInRole("Admin"))
            {
                var pendingRequirements = context.PendingRequirements.ToList();
                foreach (var requirement in pendingRequirements)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
