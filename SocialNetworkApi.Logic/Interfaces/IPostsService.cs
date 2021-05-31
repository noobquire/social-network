using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IPostsService
    {
        Task<PostDto> GetByIdAsync(string postId);
        Task<PagedResponse<PostDto>> GetByProfileAsync(string profileId, PaginationFilter filter);
        Task<PostDto> CreateAsync(string profileId, PostDataModel postData);
        Task<bool> UpdateAsync(string postId, PostDataModel postData);
        Task<bool> DeleteByIdAsync(string postId);
    }
}