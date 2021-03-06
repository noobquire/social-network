using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class PostsService : IPostsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;

        public PostsService(IUnitOfWork unitOfWork, UserManager<User> userManager, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<PostDto> GetByIdAsync(string postId)
        {
            return (await _unitOfWork.Posts.GetByIdAsync(postId))?.ToDto();
        }

        public async Task<PagedResponse<PostDto>> GetByProfileAsync(string profileId, PaginationFilter filter)
        {
            await CheckIfProfileExists(profileId);

            var allPosts = (await _unitOfWork.Posts
                .QueryAsync(p =>
                    p.ProfileId.ToString() == profileId))
                .Select(p => p.ToDto());
            return PagedResponse<PostDto>.CreatePagedResponse(allPosts, filter);
        }

        private async Task CheckIfProfileExists(string profileId)
        {
            var profile = await _unitOfWork.Profiles.GetByIdAsync(profileId);
            if (profile == null)
            {
                throw new ItemNotFoundException("Profile with such Id was not found");
            }
        }

        public async Task<PostDto> CreateAsync(string profileId, PostDataModel postData)
        {
            await CheckIfProfileExists(profileId);

            var post = new Post()
            {
                ProfileId = new Guid(profileId),
                AttachedImageId = postData.AttachedImageId == null ? null : new Guid?(new Guid(postData.AttachedImageId)),
                Text = postData.Text,
                AuthorId = await GetUserId(),
                TimePublished = DateTime.UtcNow
            };

            await _unitOfWork.Posts.CreateAsync(post);

            return post.ToDto();
        }

        private async Task<Guid> GetUserId()
        {
            var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            return user.Id;
        }

        public async Task<bool> UpdateAsync(string postId, PostDataModel postData)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(postId);

            post.Update(postData);

            return await _unitOfWork.Posts.UpdateAsync(post);
        }

        public async Task<bool> DeleteByIdAsync(string postId)
        {
            return await _unitOfWork.Posts.DeleteByIdAsync(postId);
        }
    }
}
