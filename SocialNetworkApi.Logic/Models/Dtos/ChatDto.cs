using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Models.Dtos
{
    public class ChatDto
    {
        [Editable(false)]
        public string Id { get; set; }

        [StringLength(200)]
        [Editable(true)]
        public string Name { get; set; }

        [Editable(true)]
        public bool IsDeleted { get; set; }

        [Editable(false)]
        public virtual IList<string> ParticipantIds { get; set; }

        [Editable(false)]
        public string Type { get; set; }
    }
}
