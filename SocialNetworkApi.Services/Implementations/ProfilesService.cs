using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Extensions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<bool> UpdateAsync(ProfileDto profile)
        {
            var storedProfile = await _unitOfWork.Profiles.GetByIdAsync(profile.Id);
            if (storedProfile == null)
            {
                return false;
            }
            storedProfile.Update(profile);
            return await _unitOfWork.Profiles.UpdateAsync(storedProfile);
        }

        private async Task<bool> ProfileExistsAsync(string userId)
        {
            var profiles = await _unitOfWork
                .Profiles
                .GetAllAsync();
            return profiles.Any(p => p.UserId.ToString() == userId);
        }

        public async Task<ProfileDto> GetByIdAsync(string profileId)
        {
            var profile = await _unitOfWork
                .Profiles
                .GetByIdAsync(profileId);
            return profile?.ToDto();
        }

        public async Task<PagedResponse<ProfileDto>> GetAllAsync(PaginationFilter filter)
        {
            var totalRecords = await _unitOfWork.Profiles.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize);
            var pageNumber = filter.PageNumber > totalPages ? totalPages : filter.PageNumber;
            filter.PageNumber = pageNumber;
            var pagedProfiles = await _unitOfWork.Profiles.GetPaginatedAsync(filter);
            var response = new PagedResponse<ProfileDto>(pagedProfiles.Select(p => p.ToDto()), pageNumber, filter.PageSize)
            {
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };
            return response;
        }

        public async Task<bool> DeleteByIdAsync(string profileId)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync(profileId);

            if (profile == null || profile.IsDeleted)
            {
                return false;
            }

            profile.IsDeleted = true;

            await _unitOfWork.Profiles.UpdateAsync(profile);
            return true;
        }

        public async Task<bool> ReinstateAsync(string profileId)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync(profileId);

            if (profile == null || !profile.IsDeleted)
            {
                return false;
            }

            profile.IsDeleted = false;

            await _unitOfWork.Profiles.UpdateAsync(profile);
            return true;
        }
    }
}
