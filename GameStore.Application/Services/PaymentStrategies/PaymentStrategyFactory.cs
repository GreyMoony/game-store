using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Application.Services.PaymentStrategies;
public class PaymentStrategyFactory(IServiceScopeFactory serviceScopeFactory) : IPaymentStrategyFactory
{
    public IPaymentStrategy GetStrategy(string method)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        return method switch
        {
            PaymentMethods.Visa => serviceProvider.GetRequiredService<VisaPaymentStrategy>(),
            PaymentMethods.IBox => serviceProvider.GetRequiredService<BoxPaymentStrategy>(),
            PaymentMethods.Bank => serviceProvider.GetRequiredService<BankPaymentStrategy>(),
            _ => throw new NotSupportedException($"Payment method {method} is not supported"),
        };
    }
}
