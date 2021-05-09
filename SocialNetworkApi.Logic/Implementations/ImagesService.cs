using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models.Dtos;
using SocialNetworkApi.Services.Extensions;

namespace SocialNetworkApi.Services.Implementations
{
    public class ImagesService : IImagesService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ImagesService(IHttpContextAccessor contextAccessor, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ImageHeaderDto> UploadAsync(IFormFile imageData)
        {
            var image = new Image
            {
                Name = Path.GetFileNameWithoutExtension(imageData.FileName),
                Type = GetExtension(imageData.FileName),
                Data = await GetImageData(imageData),
                OwnerId = await GetUserId()
            };

            await _unitOfWork.Images.CreateAsync(image);

            return image.ToHeaderDto();
        }

        private async Task<Guid> GetUserId()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            return user.Id;
        }

        private async Task<byte[]> GetImageData(IFormFile imageData)
        {
            await using var ms = new MemoryStream();
            await imageData.CopyToAsync(ms);
            var data = ms.ToArray();
            return data;
        }

        private ImageType GetExtension(string fileName)
        {
            var extensionString = Path.GetExtension(fileName)?.Substring(1);
            var ti = CultureInfo.InvariantCulture.TextInfo;
            var extension = (ImageType)Enum.Parse(typeof(ImageType), ti.ToTitleCase(extensionString ?? string.Empty));
            return extension;
        }

        public async Task<ImageDto> GetByIdAsync(string imageId)
        {
            return (await _unitOfWork.Images.GetByIdAsync(imageId))?.ToDto();
        }

        public async Task<ImageHeaderDto> GetHeaderByIdAsync(string imageId)
        {
            return (await _unitOfWork.Images.GetByIdAsync(imageId))?.ToHeaderDto();
        }

        public async Task<IEnumerable<ImageDto>> GetByUserAsync(string userId)
        {
            return (await _unitOfWork.Images
                    .QueryAsync(i =>
                    i.OwnerId.ToString() == userId))
                .Select(i => i.ToDto());
        }

        public async Task<bool> DeleteByIdAsync(string imageId)
        {
            return await _unitOfWork.Images.DeleteByIdAsync(imageId);
        }

        public async Task<IEnumerable<ImageHeaderDto>> GetHeadersByUserAsync(string userId)
        {
            return (await _unitOfWork.Images
                    .QueryAsync(i =>
                        i.OwnerId.ToString() == userId))
                .Select(i => i.ToHeaderDto());
        }
    }
}
