using GameStore.API.Attributes;

namespace GameStore.API.Models.UserModels;

public class CheckAccessRequest
{
    [AllowedPages]
    public string TargetPage { get; set; }

    public string? TargetId { get; set; }
}
