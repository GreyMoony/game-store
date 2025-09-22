namespace GameStore.Application.DTOs.OrderDtos;
public class PaymentIBoxDetails : PaymentDetails
{
    public Guid AccountNumber { get; set; }

    public Guid InvoiceNumber { get; set; }
}
