namespace GameStore.Domain.Entities.Mongo;
public class OrderNorthwindDetail : NorthwindEntity
{
    public int OrderID { get; set; }

    public int ProductID { get; set; }

    public double UnitPrice { get; set; }

    public int Quantity { get; set; }

    public double Discount { get; set; }
}
