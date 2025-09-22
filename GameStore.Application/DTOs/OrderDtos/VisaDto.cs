namespace GameStore.Application.DTOs.OrderDtos;
public class VisaDto
{
    public string Holder { get; set; }

    public string CardNumber { get; set; }

    public int MonthExpire { get; set; }

    public int YearExpire { get; set; }

    public int CVV2 { get; set; }
}
