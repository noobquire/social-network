using SocialNetworkApi.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents image data.
    /// </summary>
    public class Image : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name length can be no more than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(10)]
        public ImageType Type { get; set; }

        [Required(ErrorMessage = "Data is required.")]
        [MaxLength((int)10E6, ErrorMessage = "Image size can be no more than 10 MB.")]
        public byte[] Data { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

        public Guid? OwnerId { get; set; }
    }
}