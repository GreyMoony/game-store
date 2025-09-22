namespace GameStore.Application.Services.GameServices;
public static class PublishDateOptions
{
    public const string Week = "last week";
    public const string Month = "last month";
    public const string Year = "last year";
    public const string TwoYears = "2 years";
    public const string ThreeYears = "3 years";

    public static List<string> List { get; } =
        [
            Week,
            Month,
            Year,
            TwoYears,
            ThreeYears,
        ];
}
