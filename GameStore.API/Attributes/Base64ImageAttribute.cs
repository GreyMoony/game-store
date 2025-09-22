using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GameStore.API.Attributes;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
public class Base64ImageAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is not string base64String)
        {
            return false;
        }

        try
        {
            // Validate Base64 format (with optional data URI prefix)
            var match = Regex.Match(base64String, @"^data:image\/(png|jpeg|jpg|gif|bmp|webp);base64,(.+)$", RegexOptions.IgnoreCase);

            return match.Success;
        }
        catch
        {
            return false;
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid Base64-encoded image in PNG, JPEG, JPG, GIF, BMP, or WEBP format.";
    }
}
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
