using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IPostsService
    {
        Task<PostDto> GetByIdAsync(string postId);
        Task<IEnumerable<PostDto>> GetByProfileAsync(string profileId);
        Task<PostDto> CreateAsync(string profileId, PostDataModel postData);
        Task<bool> UpdateAsync(string postId, PostDataModel postData);
        Task<bool> DeleteByIdAsync(string postId);
    }
}