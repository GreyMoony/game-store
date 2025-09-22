namespace GameStore.Application.Services.NotificationServices;
public static class NotificationMethods
{
    public const string SMS = "sms";
    public const string Push = "push";
    public const string Email = "email";

    public static List<string> List { get; } =
        [
            SMS,
            Push,
            Email,
        ];
}
