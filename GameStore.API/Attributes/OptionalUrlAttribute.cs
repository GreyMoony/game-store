using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Attributes;

public class OptionalUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null
            || string.IsNullOrWhiteSpace(value.ToString())
            || string.Equals(value.ToString(), "NULL", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Success; // Allow null or empty values
        }

        if (Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeFtp))
        {
            return ValidationResult.Success; // Valid URL
        }

        return new ValidationResult("The field is not a valid URL.");
    }
}
