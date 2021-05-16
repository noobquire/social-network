using System.ComponentModel.DataAnnotations;
using SocialNetworkApi.Services.Attributes;

namespace SocialNetworkApi.Services.Models
{
    public class MessageDataModel
    {
        [Required(ErrorMessage = "Text is required.")]
        [StringLength(2000, ErrorMessage = "Max message length is 2000 characters.")]
        public string Text { get; set; }

        [ValidateGuid]
        public string ReplyToId { get; set; }
    }
}
