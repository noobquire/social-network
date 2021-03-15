using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents social network user, used for authentication.
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, ErrorMessage = "FirstName length can be no more than 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, ErrorMessage = "LastName length can be no more than 50 characters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email length can be no more than 100 characters.")]
        [EmailAddress(ErrorMessage = "Must be a valid email address.")]
        public string Email { get; set; }
        [MaxLength(50, ErrorMessage = "Max amount of chats exceeded.")]
        public List<UserChat> Chats { get; set; }
    }
}
