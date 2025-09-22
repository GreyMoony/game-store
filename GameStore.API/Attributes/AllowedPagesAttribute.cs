using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Constants;

namespace GameStore.API.Attributes;

public class AllowedPagesAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string page && Pages.AllowedPages.Contains(page);
    }
}
