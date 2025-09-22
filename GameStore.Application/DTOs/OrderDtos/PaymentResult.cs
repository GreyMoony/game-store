using GameStore.Domain.Enums;

namespace GameStore.Application.DTOs.OrderDtos;
public class PaymentResult
{
    public PaymentResultType ResultType { get; set; }

    public PaymentResponse? Response { get; set; }

    public byte[]? Invoice { get; set; }

    public string ErrorMessage { get; set; }
}
