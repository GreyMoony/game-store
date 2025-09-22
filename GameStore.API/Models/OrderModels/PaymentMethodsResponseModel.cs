namespace GameStore.API.Models.OrderModels;

public class PaymentMethodsResponseModel
{
    public IEnumerable<PaymentMethodModel> PaymentMethods { get; set; }
}
