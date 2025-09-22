namespace GameStore.Domain.Entities;
public interface ICommonGame
{
    string Name { get; set; }

    string Key { get; set; }

    double Price { get; set; }

    int UnitInStock { get; set; }

    int ViewCount { get; set; }

    DateTime? CreatedAt { get; set; }

    int? ProductID { get; set; }

    string? QuantityPerUnit { get; set; }

    int? UnitsOnOrder { get; set; }

    int? ReorderLevel { get; set; }

    bool? Discontinued { get; set; }

    int CountComments();
}
