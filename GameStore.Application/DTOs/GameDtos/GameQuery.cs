using GameStore.Application.Services.GameServices;

namespace GameStore.Application.DTOs.GameDtos;
public class GameQuery
{
#pragma warning disable IDE0028 // Simplify collection initialization
    public List<string> Genres { get; set; } = new();

    public List<string> Platforms { get; set; } = new();

    public List<string> Publishers { get; set; } = new();

    public double? MinPrice { get; set; }

    public double? MaxPrice { get; set; }

    public string? Name { get; set; }

    public string? DatePublishing { get; set; }

    public string? Sort { get; set; }

    public int Page { get; set; } = 1;

    public string? PageCount { get; set; } = PaginationOptions.Ten; // Default page size

    public string? Trigger { get; set; } // To differentiate UI triggers
}
