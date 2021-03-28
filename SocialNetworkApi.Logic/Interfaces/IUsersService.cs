using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IUsersService
    {
        Task<IdentityResult> RegisterAsync(UserRegisterModel registerModel);
        Task<JwtToken> LoginAsync(LoginModel loginModel);
        Task<UserDto> GetByIdAsync(string id);
        Task<UserDto> GetByEmailAsync(string email);
        Task<bool> DeleteByIdAsync(string id);
        Task<IEnumerable<UserDto>> GetAllAsync(bool withDeleted = false);
        Task<bool> Reinstate(string userId);
    }
}
