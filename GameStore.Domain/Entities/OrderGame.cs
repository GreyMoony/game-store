using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Domain.Entities;
public class OrderGame : BaseEntity
{
    [Column(Order = 1)]
    public Guid OrderId { get; set; }

    [Column(Order = 2)]
    public Guid ProductId { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    public int Discount { get; set; }

    public Order Order { get; set; }

    public Game Product { get; set; }
}
