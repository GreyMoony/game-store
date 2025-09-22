using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.OrderModels;

public class PaymentRequestModel
{
    [Required]
    public string Method { get; set; }

    public VisaModel? Model { get; set; }
}
