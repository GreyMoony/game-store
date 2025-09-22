using System.ComponentModel.DataAnnotations;
using GameStore.API.Models.CommentModels;
using GameStore.Application.Services.CommentServices;

namespace GameStore.API.Attributes;

public class ParentIdValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var model = (AddCommentModel)validationContext.ObjectInstance;

        if (model.Action is CommentActions.Quote or CommentActions.Reply)
        {
            // Validate that ParentId is not null
            if (!model.ParentId.HasValue)
            {
                return new ValidationResult("ParentId is required for Quote or Reply actions.");
            }
        }
        else if (string.IsNullOrEmpty(model.Action) && model.ParentId.HasValue)
        {
            return new ValidationResult("ParentId must be empty when Action is empty.");
        }

        return ValidationResult.Success;
    }
}
