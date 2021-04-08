using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IImagesService
    {
        Task<ImageHeaderDto> UploadAsync(IFormFile imageData);
        Task<ImageDto> GetByIdAsync(string imageId);
        Task<ImageHeaderDto> GetHeaderByIdAsync(string imageId);
        Task<IEnumerable<ImageDto>> GetByUserAsync(string userId);
        Task<bool> DeleteByIdAsync(string imageId);
        Task<IEnumerable<ImageHeaderDto>> GetHeadersByUserAsync(string userId);
    }
}