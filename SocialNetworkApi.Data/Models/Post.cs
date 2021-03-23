using System;
using System.ComponentModel.DataAnnotations;
using SocialNetworkApi.Data.Interfaces;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a post in user's profile.
    /// </summary>
    public class Post : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public User Author { get; set; }
        [Required(ErrorMessage = "Author is required")]
        public Guid AuthorId { get; set; }
        public Profile Profile { get; set; }
        [Required(ErrorMessage = "Profile is required")]
        public Guid ProfileId { get; set; }
        public Image AttachedImage { get; set; }
        [Required(ErrorMessage = "Text is required")]
        [StringLength(2000, ErrorMessage = "Text is required.")]
        public string Text { get; set; }
        public DateTime TimePublished { get; set; }
    }
}
