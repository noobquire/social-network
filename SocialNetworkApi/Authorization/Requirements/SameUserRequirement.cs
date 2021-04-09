using Microsoft.AspNetCore.Authorization;

namespace SocialNetworkApi.Authorization.Requirements
{
    public class SameUserRequirement : IAuthorizationRequirement
    {
        public bool AllowAdmin { get; }

        public SameUserRequirement(bool allowAdmin)
        {
            AllowAdmin = allowAdmin;
        }
    }
}
