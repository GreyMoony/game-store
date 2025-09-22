namespace GameStore.Application.DTOs.UserDtos;
public class AuthErrorResponse
{
    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int Status { get; set; }

    public string Detail { get; set; } = string.Empty;

    public string Instance { get; set; } = string.Empty;

    public string AdditionalProp1 { get; set; }

    public string AdditionalProp2 { get; set; }

    public string AdditionalProp3 { get; set; }
}
