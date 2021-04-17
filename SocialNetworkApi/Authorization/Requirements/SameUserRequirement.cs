using Microsoft.AspNetCore.Authorization;

namespace SocialNetworkApi.Authorization.Requirements
{
    /// <summary>
    /// Indicates that same user who created this resource, or admin, can access it
    /// </summary>
    public class SameUserRequirement : IAuthorizationRequirement
    {
        public bool AllowAdmin { get; }

        public SameUserRequirement(bool allowAdmin)
        {
            AllowAdmin = allowAdmin;
        }
    }
}
