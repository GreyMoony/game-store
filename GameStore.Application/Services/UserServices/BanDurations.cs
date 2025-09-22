namespace GameStore.Application.Services.UserServices;
public static class BanDurations
{
    public const string Hour = "1 hour";
    public const string Day = "1 day";
    public const string Week = "1 week";
    public const string Month = "1 month";
    public const string Permanent = "permanent";

    public static List<string> List { get; } =
        [
            Hour,
            Day,
            Week,
            Month,
            Permanent,
        ];

    public static TimeSpan? GetBanDurationSpan(string duration)
    {
        return duration switch
        {
            Hour => TimeSpan.FromHours(1),
            Day => TimeSpan.FromDays(1),
            Week => TimeSpan.FromDays(7),
            Month => TimeSpan.FromDays(30),
            Permanent => null,
            _ => throw new ArgumentException("Invalid duration"),
        };
    }
}
