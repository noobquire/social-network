namespace SocialNetworkApi.Services.Models.Dtos
{
    public class ImageDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public byte[] Data { get; set; }

        public string OwnerId { get; set; }
    }
}