using System;
using System.Linq;
using System.Threading.Tasks;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Extensions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Implementations
{
    public class ProfilesService : IProfilesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfilesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileDto> CreateAsync(string userId)
        {
            if (await ProfileExistsAsync(userId))
            {
                throw new ProfileAlreadyExistsException("User with such Id already has a profile.");
            }

            var profile = new Profile
            {
                UserId = new Guid(userId),
            };

            await _unitOfWork.Profiles.CreateAsync(profile);

            return profile.ToDto();
        }

        private async Task<bool> ProfileExistsAsync(string userId)
        {
            var profiles = await _unitOfWork
                .Profiles
                .GetAllAsync();
            return profiles.Any(p => p.UserId.ToString() == userId);
        }

        public async Task<bool> EditAsync(ProfileDto profile)
        {
            throw new NotImplementedException();
        }

        public async Task<ProfileDto> GetByIdAsync(string profileId)
        {
            var profile = await _unitOfWork
                .Profiles
                .GetByIdAsync(profileId);
            return profile?.ToDto();
        }

        public async Task<bool> DeleteByIdAsync(string profileId)
        {
            throw new NotImplementedException();
        }
    }
}
