using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Extensions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

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

        public async Task<IEnumerable<PostDto>> GetByProfileAsync(string profileId)
        {
            return (await _unitOfWork.Posts
                .QueryAsync(p => 
                    p.ProfileId.ToString() == profileId))
                .Select(p => p.ToDto());
        }

        public async Task<PostDto> CreateAsync(string profileId, PostDataModel postData)
        {
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
