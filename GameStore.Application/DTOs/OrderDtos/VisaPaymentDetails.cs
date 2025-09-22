namespace GameStore.Application.DTOs.OrderDtos;
public class VisaPaymentDetails : PaymentDetails
{
    public string CardHolderName { get; set; }

    public string CardNumber { get; set; }

    public int ExpirationMonth { get; set; }

    public int ExpirationYear { get; set; }

    public int CVV { get; set; }
}
