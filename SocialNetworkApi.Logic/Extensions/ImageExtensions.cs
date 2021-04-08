using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Extensions
{
    public static class ImageExtensions
    {
        public static ImageDto ToDto(this Image image)
        {
            if (image == null) return null;
            return new ImageDto
            {
                Id = image.Id.ToString(),
                Name = image.Name,
                Extension = image.Extension.ToString().ToLowerInvariant(),
                Data = image.Data
            };
        }

        public static ImageHeaderDto ToHeaderDto(this Image image)
        {
            if (image == null) return null;
            return new ImageHeaderDto
            {
                Id = image.Id.ToString(),
                Name = image.Name,
                Extension = image.Extension.ToString().ToLowerInvariant(),
                Size = image.Data.LongLength.ToString()
            };
        }
    }
}
