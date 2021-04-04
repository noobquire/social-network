using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IProfilesService
    {
        public Task<ProfileDto> CreateAsync(string userId);
        public Task<bool> UpdateAsync(ProfileDto profile);
        public Task<ProfileDto> GetByIdAsync(string profileId);
        public Task<bool> DeleteByIdAsync(string profileId);
        public Task<bool> ReinstateAsync(string profileId);
    }
}
