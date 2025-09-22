using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameStore.Application.DTOs.NotificationDtos;
using GameStore.Application.Services.EmailSender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace GameStore.Application.Services.NotificationServices;
public class EmailNotificationBackgroundService(
    ServiceBusClient client,
    IConfiguration config,
    IEmailSender emailSender) : BackgroundService
{
    private readonly ServiceBusProcessor _processor = client.CreateProcessor(config["AzureServiceBus:QueueName"]);
    private readonly IEmailSender _emailSender = emailSender;

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();

            EmailNotificationDto? email = null;

            try
            {
                email = JsonSerializer.Deserialize<EmailNotificationDto>(body);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Invalid email payload: {ex.Message}");
                await args.DeadLetterMessageAsync(
                    args.Message, "Deserialization failed", ex.Message);
                return;
            }

            if (email is null)
            {
                Console.WriteLine("EmailNotificationDto was null after deserialization.");
                await args.DeadLetterMessageAsync(
                    args.Message, "Deserialization resulted in null", "Invalid payload format.");
                return;
            }

            await _emailSender.SendEmailAsync(email);
            await args.CompleteMessageAsync(args.Message);
        };

        _processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        };

        return _processor.StartProcessingAsync(stoppingToken);
    }
}
