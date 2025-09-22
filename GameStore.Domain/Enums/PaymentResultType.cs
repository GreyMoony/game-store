namespace GameStore.Domain.Enums;
public enum PaymentResultType
{
    PaymentSuccess,   // Indicates payment was successful
    InvoiceGenerated,     // Indicates a PDF was generated
    PaymentFailure,   // Indicates payment failed
}
