using System.Net.Http.Json;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameStore.Application.Services.PaymentStrategies;
public class BoxPaymentStrategy(IHttpClientFactory httpClientFactory,
    ILogger<VisaPaymentStrategy> logger,
    IOptions<PaymentServiceSettings> options) : PaymentStrategyBase(logger)
{
    public override async Task<PaymentResult> ProcessPaymentAsync(Order cart, PaymentRequestDto request)
    {
        var iboxRequest = new PaymentIBoxDetails
        {
            AccountNumber = cart.CustomerId,
            InvoiceNumber = cart.Id,
            TransactionAmount = cart
            .OrderGames
            .Sum(og => og.Price * og.Quantity * (100 - og.Discount) / 100),
        };

        var retryPolicy = ConfigurePolicy("IBox terminal");

        var client = httpClientFactory.CreateClient();
        var response = await retryPolicy.ExecuteAsync(
            () => client.PostAsJsonAsync(options.Value.IBoxRequestUri, iboxRequest));

        if (response.IsSuccessStatusCode)
        {
            return new PaymentResult
            {
                ResultType = PaymentResultType.PaymentSuccess,
                Response = new PaymentResponse
                {
                    UserId = iboxRequest.AccountNumber,
                    OrderId = iboxRequest.InvoiceNumber,
                    PaymentDate = DateTime.UtcNow,
                    Sum = iboxRequest.TransactionAmount,
                },
                ErrorMessage = null,
            };
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogWarning("Visa payment failed: {ErrorContent}", errorContent);

            return new PaymentResult
            {
                ResultType = PaymentResultType.PaymentFailure,
                ErrorMessage = errorContent,
            };
        }
    }
}
