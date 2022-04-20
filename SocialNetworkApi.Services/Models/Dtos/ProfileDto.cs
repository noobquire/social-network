using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class ProfileDto
    {
        [Editable(false)]
        public string Id { get; set; }
        [Editable(true)]
        [StringLength(200, ErrorMessage = "Status length can be no more than 50 characters.")]
        public string Status { get; set; }
        [Editable(true)]
        public string AvatarId { get; set; }
        [Editable(false)]
        public string UserId { get; set; }
    }
}
