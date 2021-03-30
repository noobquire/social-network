using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class ProfileDto
    {
        public string Id { get; set; }
        [StringLength(200, ErrorMessage = "Status length can be no more than 50 characters.")]
        public string Status { get; set; }
        public string AvatarId { get; set; }
        public string UserId { get; set; }
    }
}
