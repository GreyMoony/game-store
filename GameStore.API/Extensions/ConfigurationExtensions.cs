using GameStore.Domain.Settings;

namespace GameStore.API.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PaymentServiceSettings>(config.GetSection("PaymentService"));
        services.Configure<AuthServiceSettings>(config.GetSection("AuthService"));
        services.Configure<InvoiceSettings>(config.GetSection("InvoiceSettings"));
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.Configure<MongoDbSettings>(config.GetSection("MongoDB"));
        services.Configure<SendGridSettings>(config.GetSection("SendGrid"));

        return services;
    }
}
