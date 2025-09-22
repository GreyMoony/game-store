using GameStore.API.Attributes;
using GameStore.Application.Services.GameServices;

namespace GameStore.API.Models.GameModels;

public class GameQueryModel
{
#pragma warning disable IDE0028 // Simplify collection initialization
    public List<string> Genres { get; set; } = new();

    public List<string> Platforms { get; set; } = new();

    public List<string> Publishers { get; set; } = new();

    public double? MinPrice { get; set; }

    public double? MaxPrice { get; set; }

    public string? Name { get; set; }

    [NullableAllowedValues(
        PublishDateOptions.Week,
        PublishDateOptions.Month,
        PublishDateOptions.Year,
        PublishDateOptions.TwoYears,
        PublishDateOptions.ThreeYears)]
    public string? DatePublishing { get; set; }

    [NullableAllowedValues(
        SortingOptions.PriceDesc,
        SortingOptions.PriceAsc,
        SortingOptions.MostCommented,
        SortingOptions.MostPopular,
        SortingOptions.Newest)]
    public string? Sort { get; set; }

    public int Page { get; set; } = 1;

    [NullableAllowedValues(
        PaginationOptions.Ten,
        PaginationOptions.Twenty,
        PaginationOptions.Fifty,
        PaginationOptions.Hundred,
        PaginationOptions.All)]
    public string? PageCount { get; set; } = PaginationOptions.Ten; // Default page size

    public string? Trigger { get; set; }
}
