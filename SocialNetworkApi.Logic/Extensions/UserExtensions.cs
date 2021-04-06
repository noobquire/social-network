using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id.ToString(),
                Email = user.Email,
                ProfileId = user.ProfileId.ToString(),
                Username = user.UserName
            };
        }
    }
}
