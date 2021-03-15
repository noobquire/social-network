using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a message sent to chat.
    /// </summary>
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public User Author { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public Guid AuthorId { get; set; }
        [Required(ErrorMessage = "Text is required.")]
        [StringLength(2000, ErrorMessage = "Max message length is 2000 characters.")]
        public string Text { get; set; }
        public DateTime TimePublished { get; set; }
        public Chat Chat { get; set; }
        [Required(ErrorMessage = "Chat is required.")]
        public Guid ChatId { get; set; }
        public Message ReplyTo { get; set; }
    }
}
