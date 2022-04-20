using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SocialNetworkApi.Services.Extensions
{
    public static class ImageExtensions
    {
        public static ImageDto ToDto([NotNull] this Image image)
        {
            return new ImageDto
            {
                Id = image.Id.ToString(),
                Name = image.Name,
                Extension = image.Type.ToString().ToLowerInvariant(),
                Data = image.Data,
                OwnerId = image.OwnerId.ToString()
            };
        }

        public static ImageHeaderDto ToHeaderDto([NotNull] this Image image)
        {
            return new ImageHeaderDto
            {
                Id = image.Id.ToString(),
                Name = image.Name,
                Extension = image.Type.ToString().ToLowerInvariant(),
                Size = image.Data.LongLength.ToString(),
                OwnerId = image.OwnerId.ToString()
            };
        }
    }
}
