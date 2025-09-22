namespace GameStore.Application.DTOs.OrderDtos;
public class PaymentRequestDto
{
    public string Method { get; set; }

    public VisaDto? Model { get; set; }
}
