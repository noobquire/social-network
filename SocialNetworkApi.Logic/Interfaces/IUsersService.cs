using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UserDto> RegisterAsync(UserRegisterModel registerModel);
        Task<JwtToken> LoginAsync(LoginModel loginModel);
        Task<UserDto> GetByIdAsync(string id);
        Task<UserDto> GetByEmailAsync(string email);
        Task<bool> DeleteByIdAsync(string id);
        Task<PagedResponse<UserDto>> GetAllAsync(PaginationFilter filter);
        Task<bool> ReinstateAsync(string userId);
    }
}
