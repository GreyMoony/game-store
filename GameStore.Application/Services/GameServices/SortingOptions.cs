namespace GameStore.Application.Services.GameServices;
public static class SortingOptions
{
    public const string MostPopular = "Most popular";
    public const string MostCommented = "Most commented";
    public const string PriceAsc = "Price ASC";
    public const string PriceDesc = "Price DESC";
    public const string Newest = "New";

    public static List<string> List { get; } =
        [
            MostPopular,
            MostCommented,
            PriceAsc,
            PriceDesc,
            Newest,
        ];
}
