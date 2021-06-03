using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents social network user, used for authentication.
    /// </summary>
    public class User : IdentityUser<Guid>, IEntity
    {
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, ErrorMessage = "FirstName length can be no more than 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, ErrorMessage = "LastName length can be no more than 50 characters.")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Max amount of chats exceeded.")]
        public virtual List<UserChat> Chats { get; set; } = new List<UserChat>();
        public virtual List<Message> Messages { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<Image> Images { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
