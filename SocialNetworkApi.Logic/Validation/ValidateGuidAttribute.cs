using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetworkApi.Services.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ValidateGuidAttribute : ValidationAttribute
    {
        private readonly bool _required;
        public ValidateGuidAttribute(bool required = true)
        {
            _required = required;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null && !_required)
            {
                return ValidationResult.Success;
            }

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
