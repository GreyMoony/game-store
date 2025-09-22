using System.Net.Http.Json;
using AutoMapper;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameStore.Application.Services.PaymentStrategies;
public class VisaPaymentStrategy(IHttpClientFactory httpClientFactory,
    IMapper mapper,
    ILogger<VisaPaymentStrategy> logger,
    IOptions<PaymentServiceSettings> options) : PaymentStrategyBase(logger)
{
    public override async Task<PaymentResult> ProcessPaymentAsync(Order cart, PaymentRequestDto request)
    {
        var visaRequest = mapper.Map<VisaPaymentDetails>(request.Model);
        visaRequest.TransactionAmount = cart
            .OrderGames
            .Sum(og => og.Price * og.Quantity * (100 - og.Discount) / 100);

        var retryPolicy = ConfigurePolicy("Visa");

        var client = httpClientFactory.CreateClient();

        // Execute the request with retry logic
        var response = await retryPolicy.ExecuteAsync(
            () => client.PostAsJsonAsync(options.Value.VisaRequestUri, visaRequest));

        // Validate the response
        if (response.IsSuccessStatusCode)
        {
            return new PaymentResult
            {
                ResultType = PaymentResultType.PaymentSuccess,
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
