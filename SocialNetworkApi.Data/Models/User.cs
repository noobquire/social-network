using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents social network user, used for authentication.
    /// </summary>
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, ErrorMessage = "FirstName length can be no more than 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, ErrorMessage = "LastName length can be no more than 50 characters.")]
        public string LastName { get; set; }
        [MaxLength(50, ErrorMessage = "Max amount of chats exceeded.")]
        public List<UserChat> Chats { get; set; }
        public List<Message> Messages { get; set; }
        public List<Post> Posts { get; set; }
    }
}
