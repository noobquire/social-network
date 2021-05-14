using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class ChatDto
    {
        public string Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public virtual IList<string> ParticipantIds { get; set; }

        public string Type { get; set; }
    }
}
