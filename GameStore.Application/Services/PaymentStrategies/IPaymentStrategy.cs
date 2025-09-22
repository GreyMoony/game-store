using GameStore.Application.DTOs.OrderDtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.PaymentStrategies;
public interface IPaymentStrategy
{
    Task<PaymentResult> ProcessPaymentAsync(Order cart, PaymentRequestDto request);
}
