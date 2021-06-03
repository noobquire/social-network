using Microsoft.AspNetCore.Http;
using SocialNetworkApi.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace SocialNetworkApi.Services.Validation
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (!(value is IFormFile file))
            {
                return new ValidationResult("Specified parameter is not IFormFile");
            }

            var extensions = Enum.GetNames(typeof(ImageType)).Select(e => e.ToLowerInvariant());

            var extension = Path.GetExtension(file.FileName)?.Substring(1);
            return !extensions.Contains(extension?.ToLowerInvariant()) ?
                new ValidationResult(GetErrorMessage())
                : ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            var availableExtensions = string.Join(' ', Enum.GetNames(typeof(ImageType)).Select(e => e.ToLowerInvariant()));
            return $"Image extension is not supported. Available extensions: {availableExtensions}";
        }
    }
}