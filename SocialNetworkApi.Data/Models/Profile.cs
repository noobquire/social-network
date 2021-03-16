using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents user profile data.
    /// </summary>
    public class Profile : Entity
    {
        [StringLength(200, ErrorMessage = "Status length can be no more than 50 characters.")]
        public string Status { get; set; }
        public List<Post> Posts { get; set; }
        public Image Avatar { get; set; }
    }
}