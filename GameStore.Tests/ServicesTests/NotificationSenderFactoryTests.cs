using Azure.Messaging.ServiceBus;
using GameStore.Application.Services.NotificationServices;
using GameStore.Application.Services.NotificationServices.NotificationStrategies;
using GameStore.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GameStore.Tests.ServicesTests;

public class NotificationSenderFactoryTests
{
    private readonly NotificationSenderFactory _factory;

    public NotificationSenderFactoryTests()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var configurationMock = new Mock<IConfiguration>();
        var emailSender = new EmailNotificationSender(serviceBusClientMock.Object, configurationMock.Object);
        var smsSender = new SmsNotificationSender();
        var pushSender = new PushNotificationSender();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(EmailNotificationSender))).Returns(emailSender);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(SmsNotificationSender))).Returns(smsSender);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(PushNotificationSender))).Returns(pushSender);
        _factory = new NotificationSenderFactory(serviceProviderMock.Object);
    }

    [Theory]
    [InlineData(NotificationMethodType.Email, typeof(EmailNotificationSender))]
    [InlineData(NotificationMethodType.Sms, typeof(SmsNotificationSender))]
    [InlineData(NotificationMethodType.Push, typeof(PushNotificationSender))]
    public void GetSender_ReturnsCorrectSenderType(NotificationMethodType methodType, Type expectedType)
    {
        // Act
        var sender = _factory.GetSender(methodType);

        // Assert
        Assert.IsType(expectedType, sender);
    }

    [Fact]
    public void GetSender_ThrowsForUnknownType()
    {
        // Act & Assert
        Assert.Throws<NotSupportedException>(() => _factory.GetSender((NotificationMethodType)999));
    }
}
