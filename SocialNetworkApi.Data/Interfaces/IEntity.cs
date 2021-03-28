using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Data.Interfaces
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
