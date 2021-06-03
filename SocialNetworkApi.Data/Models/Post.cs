using SocialNetworkApi.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a post in user's profile.
    /// </summary>
    public class Post : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        public virtual User Author { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }

        public virtual Profile Profile { get; set; }

        [Required(ErrorMessage = "Profile is required")]
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        [ForeignKey("AttachedImageId")]
        public virtual Image AttachedImage { get; set; }

        public Guid? AttachedImageId { get; set; }

        [Required(ErrorMessage = "Text is required")]
        [StringLength(2000, ErrorMessage = "Max length exceeded")]
        public string Text { get; set; }

        public DateTime TimePublished { get; set; }
    }
}
