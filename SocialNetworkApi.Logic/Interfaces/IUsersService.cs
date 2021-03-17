using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<IdentityResult> RegisterAsync(UserRegisterModel registerModel);
        public Task<JwtToken> LoginAsync(LoginModel loginModel);
    }
}
