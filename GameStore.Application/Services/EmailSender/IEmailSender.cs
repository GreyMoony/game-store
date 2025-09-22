using GameStore.Application.DTOs.NotificationDtos;

namespace GameStore.Application.Services.EmailSender;
public interface IEmailSender
{
    Task SendEmailAsync(EmailNotificationDto dto);
}
