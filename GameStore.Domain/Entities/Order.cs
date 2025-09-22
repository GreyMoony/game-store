using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Enums;

namespace GameStore.Domain.Entities;
public class Order : BaseEntity
{
    public DateTime Date { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public int? OrderID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    public double? Freight { get; set; }

    public string? ShipName { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipCity { get; set; }

    public string? ShipRegion { get; set; }

    public string? ShipPostalCode { get; set; }

    public string? ShipCountry { get; set; }

    public ICollection<OrderGame> OrderGames { get; set; }
}
