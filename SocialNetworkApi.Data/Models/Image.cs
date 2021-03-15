using System;

namespace SocialNetworkApi.Data.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Data { get; set; }
    }
}