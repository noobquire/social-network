using System;
using System.ComponentModel.DataAnnotations;
using SocialNetworkApi.Data.Interfaces;

namespace SocialNetworkApi.Data.Models
{
    /// <summary>
    /// Represents image data.
    /// </summary>
    public class Image : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name length can be no more than 50 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Extension is required.")]
        [StringLength(10, ErrorMessage = "Extension length can be no more than 50 characters.")]
        // TODO: Validate if extension is supported
        public string Extension { get; set; }
        [Required(ErrorMessage = "Data is required.")]
        [MaxLength((int)10E6, ErrorMessage = "Image size can be no more than 10 MB.")]
        public byte[] Data { get; set; }
    }
}