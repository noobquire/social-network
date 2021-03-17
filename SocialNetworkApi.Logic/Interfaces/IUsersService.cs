using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<IdentityResult> RegisterAsync(UserRegisterModel registerModel);
        public Task<JwtToken> LoginAsync(LoginModel loginModel);
        public Task<UserDto> GetByIdAsync(string id);
        public Task<UserDto> GetByEmailAsync(string email);
    }
}
