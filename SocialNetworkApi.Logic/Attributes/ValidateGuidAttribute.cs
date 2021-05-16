using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ValidateGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string id))
            {
                return new ValidationResult("Input value does not contain valid GUID");
            }

            try
            {
                var guid = new Guid(id);
            }
            catch (FormatException)
            {
                return new ValidationResult("Input value does not contain valid GUID");
            }

            return ValidationResult.Success;
        }
    }
}
