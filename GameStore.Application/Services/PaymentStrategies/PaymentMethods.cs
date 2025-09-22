using GameStore.Application.DTOs.OrderDtos;

namespace GameStore.Application.Services.PaymentStrategies;
public static class PaymentMethods
{
    public const string Visa = "Visa";

    public const string Bank = "Bank";

    public const string IBox = "IBox terminal";

    public static IEnumerable<PaymentMethodDto> List { get; } =
        [
            new()
            {
                Title = Bank,
                ImageUrl = "https://img.icons8.com/?size=100&id=77051&format=png&color=000000",
                Description = "Bank payment method",
            },
            new()
            {
                Title = Visa,
                ImageUrl = "https://img.icons8.com/?size=100&id=1429&format=png&color=000000",
                Description = "Visa payment method",
            },
            new()
            {
                Title = IBox,
                ImageUrl = "https://img.icons8.com/?size=100&id=gbKZLyA9eGde&format=png&color=000000",
                Description = "IBox terminal payment method",
            },
        ];

    public static bool IsSupported(string paymentMethod)
    {
        return List.Any(m => m.Title == paymentMethod);
    }
}
