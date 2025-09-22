using System.Net;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Domain.Entities;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace GameStore.Application.Services.PaymentStrategies;
public abstract class PaymentStrategyBase(ILogger logger) : IPaymentStrategy
{
    private const int MaxRetries = 3;

    public abstract Task<PaymentResult> ProcessPaymentAsync(Order cart, PaymentRequestDto request);

    protected AsyncRetryPolicy<HttpResponseMessage> ConfigurePolicy(string paymentMethod)
    {
        return Policy
            .Handle<HttpRequestException>() // Handle HTTP request exceptions
            .OrResult<HttpResponseMessage>(response =>
                !response.IsSuccessStatusCode &&
                response.StatusCode is
                HttpStatusCode.PaymentRequired or HttpStatusCode.BadRequest)
            .WaitAndRetryAsync(
                MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2), // Exponential backoff
                (response, delay, retryCount, context) =>
                {
                    // Log the retry attempt
                    logger.LogWarning(
                        "Retrying {PaymentMethod} payment... Attempt #{RetryCount}. Status: {StatusCode}",
                        paymentMethod,
                        retryCount,
                        response?.Result?.StatusCode);
                });
    }
}
