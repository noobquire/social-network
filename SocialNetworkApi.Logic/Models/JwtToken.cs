using System;

namespace SocialNetworkApi.Services.Models
{
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
