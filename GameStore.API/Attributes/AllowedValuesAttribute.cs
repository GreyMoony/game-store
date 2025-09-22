using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Attributes;

public class AllowedValuesAttribute(params string[] allowedValues) : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue && allowedValues.Contains(strValue))
        {
            return ValidationResult.Success;
        }

        var errorMessage = $"The value '{value}' is not valid. Allowed values are: {string.Join(", ", allowedValues)}.";
        return new ValidationResult(errorMessage);
    }
}
