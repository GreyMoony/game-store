using System.ComponentModel.DataAnnotations;

namespace GameStore.API.Models.OrderModels;

public class OrderDetailQuantityUpdateRequest
{
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Count { get; set; }
}
