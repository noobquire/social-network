using Microsoft.AspNetCore.Http;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IImagesService
    {
        Task<ImageHeaderDto> UploadAsync(IFormFile imageData);
        Task<ImageDto> GetByIdAsync(string imageId);
        Task<ImageHeaderDto> GetHeaderByIdAsync(string imageId);
        Task<PagedResponse<ImageDto>> GetByUserAsync(string userId, PaginationFilter filter);
        Task<bool> DeleteByIdAsync(string imageId);
        Task<PagedResponse<ImageHeaderDto>> GetHeadersByUserAsync(string userId, PaginationFilter filter);
    }
}