namespace GameStore.Application.Services.GameServices;
public static class PaginationOptions
{
    public const string Ten = "10";
    public const string Twenty = "20";
    public const string Fifty = "50";
    public const string Hundred = "100";
    public const string All = "all";

    public static List<string> List { get; } =
        [
            Ten,
            Twenty,
            Fifty,
            Hundred,
            All,
        ];
}
