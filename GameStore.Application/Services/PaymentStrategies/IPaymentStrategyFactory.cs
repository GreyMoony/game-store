namespace GameStore.Application.Services.PaymentStrategies;
public interface IPaymentStrategyFactory
{
    IPaymentStrategy GetStrategy(string method);
}
