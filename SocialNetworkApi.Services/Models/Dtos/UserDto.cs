namespace SocialNetworkApi.Services.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
