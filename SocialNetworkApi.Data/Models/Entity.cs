using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Models
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
