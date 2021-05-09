using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialNetworkApi.Data.Interfaces;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents a personal or group chat with several participants
    /// </summary>
    public class Chat : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "At least two participants are required.")]
        [MinLength(2, ErrorMessage = "At least two participants are required.")]
        [MaxLength(50, ErrorMessage = "No more than 50 participants are allowed.")]
        public virtual List<UserChat> Participants { get; set; }

        public virtual List<Message> Messages { get; set; }

        public ChatType Type { get; set; }
    }
}