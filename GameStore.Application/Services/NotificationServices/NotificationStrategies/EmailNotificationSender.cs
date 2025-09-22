using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameStore.Application.DTOs.NotificationDtos;
using GameStore.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace GameStore.Application.Services.NotificationServices.NotificationStrategies;
public class EmailNotificationSender(
    ServiceBusClient client,
    IConfiguration config) : INotificationSender
{
    // Hardcoded email until added user email registration
    // Change to your email to receive notifications for testing purposes
    private const string _defaultEmail = "sergvoy1996@gmail.com";

    private readonly ServiceBusSender _sender = client.CreateSender(config["AzureServiceBus:QueueName"]);

    public async Task SendAsync(ApplicationUser user, string message, Order order)
    {
        var payload = JsonSerializer.Serialize(new EmailNotificationDto
        {
            UserId = user.Id,
            Message = message,
            Email = user.Email ?? _defaultEmail,
            OrderId = order.Id.ToString(),
            Status = order.Status.ToString(),
            Items =
                [.. order.OrderGames.Select(og => new EmailNotificationItemDto
                {
                    GameTitle = og.Product.Name,
                    Quantity = og.Quantity,
                    UnitPrice = (decimal)og.Price,
                })],
            TotalPrice = (decimal)order.OrderGames.Sum(og => og.Quantity * og.Price),
        });

        await _sender.SendMessageAsync(new ServiceBusMessage(payload));
    }
}
