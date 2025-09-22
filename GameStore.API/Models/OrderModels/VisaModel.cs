using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.OrderModels;

public class VisaModel
{
    [Required(ErrorMessage = "Holder Name is required")]
    public string Holder { get; set; }

    [Required(ErrorMessage = "Card number is required")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Month of expiration is required")]
    public int MonthExpire { get; set; }

    [Required(ErrorMessage = "Year of expiration is required")]
    public int YearExpire { get; set; }

    [Required(ErrorMessage = "CVV2 is required")]
    [Range(1, 999, ErrorMessage = "CVV2 must be from 1 to 999")]
    public int CVV2 { get; set; }
}
