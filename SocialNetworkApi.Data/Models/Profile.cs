using SocialNetworkApi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents user profile data.
    /// </summary>
    public class Profile : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(200, ErrorMessage = "Status length can be no more than 50 characters.")]
        public string Status { get; set; }

        public virtual List<Post> Posts { get; set; }

        [ForeignKey("AvatarId")]
        public virtual Image Avatar { get; set; }

        public Guid? AvatarId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public Guid UserId { get; set; }
    }
}