using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Models
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, ErrorMessage = "FirstName length can be no more than 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, ErrorMessage = "LastName length can be no more than 50 characters.")]
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
