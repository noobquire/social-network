using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Models
{
    /// <summary>
    /// Model for creating a new group chat
    /// </summary>
    public class NewChatModel
    {
        /// <summary>
        /// List containing user IDs of participants of new group chat, not including creator
        /// </summary>
        [Required]
        [MaxLength(49)]
        public IEnumerable<string> ParticipantIds { get; set; }
        /// <summary>
        /// Group chat name
        /// </summary>
        [StringLength(200)]
        public string Name { get; set; }
    }
}
