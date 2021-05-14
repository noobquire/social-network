using System;
using System.ComponentModel.DataAnnotations;
using SocialNetworkApi.Data.Interfaces;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a message sent to chat.
    /// </summary>
    public class Message : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }
        public virtual User Author { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public Guid AuthorId { get; set; }
        [Required(ErrorMessage = "Text is required.")]
        [StringLength(2000, ErrorMessage = "Max message length is 2000 characters.")]
        public string Text { get; set; }
        public DateTime TimePublished { get; set; }
        public virtual Chat Chat { get; set; }
        [Required(ErrorMessage = "Chat is required.")]
        public Guid ChatId { get; set; }
        public virtual Message ReplyTo { get; set; }
    }
}
