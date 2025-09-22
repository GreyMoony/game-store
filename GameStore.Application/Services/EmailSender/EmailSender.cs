using System.Text;
using DnsClient.Internal;
using GameStore.Application.DTOs.NotificationDtos;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GameStore.Application.Services.EmailSender;
public class EmailSender(
    SendGridClient client,
    IOptions<SendGridSettings> options,
    ILogger<EmailSender> logger) : IEmailSender
{
    private readonly string _fromEmail = options.Value.FromEmail;

    public async Task SendEmailAsync(EmailNotificationDto dto)
    {
        var subject = $"Your order {dto.OrderId} status changed to: {dto.Status}";
        var from = new EmailAddress(_fromEmail, "GameStore Notifications");
        var to = new EmailAddress(dto.Email);

        var html = BuildHtmlBody(dto);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: html);
        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync();
            logger.LogError("Failed to send email: {Body}", body);
        }
    }

    private static string BuildHtmlBody(EmailNotificationDto dto)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"<h2>{dto.Message}</h2>");
        builder.AppendLine("<ul>");
        foreach (var item in dto.Items)
        {
            builder.AppendLine($"<li>{item.GameTitle} - {item.Quantity} × {item.UnitPrice:C}</li>");
        }

        builder.AppendLine("</ul>");
        builder.AppendLine($"<p>Total Price: {dto.TotalPrice:C}</p>");
        return builder.ToString();
    }
}
