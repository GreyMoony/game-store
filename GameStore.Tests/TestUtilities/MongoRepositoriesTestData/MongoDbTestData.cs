using GameStore.Domain.Entities.Mongo;

namespace GameStore.Tests.TestUtilities.MongoRepositoriesTestData;
public static class MongoDbTestData
{
    public static List<Category> Categories =>
        [
            new Category { CategoryID = 1, CategoryName = "Beverages" },
            new Category { CategoryID = 2, CategoryName = "Condiments", IsDeleted = true },
            new Category { CategoryID = 3, CategoryName = "Sweets" },
            new Category { CategoryID = 4, CategoryName = "Drinks" }
        ];

    public static List<Product> Products =>
        [
            new Product { ProductID = 1, CategoryID = 1, Key = "beer", SupplierID = 1 },
            new Product { ProductID = 2, CategoryID = 2, Key = "ketchup", IsDeleted = true, SupplierID = 2 },
            new Product { ProductID = 3, CategoryID = 3, Key = "chocolate", SupplierID = 3 },
            new Product { ProductID = 4, CategoryID = 4, Key = "juice", SupplierID = 3 }
        ];

    public static List<Supplier> Suppliers =>
        [
            new Supplier { SupplierID = 1, CompanyName = "Supplier A" },
            new Supplier { SupplierID = 2, CompanyName = "Supplier B", IsDeleted = true },
            new Supplier { SupplierID = 3, CompanyName = "Supplier C" }
        ];

    public static List<OrderNorthwind> Orders =>
        [
            new OrderNorthwind { OrderID = 1, CustomerID = "CUST1", OrderDate = DateTime.Now.ToString() },
            new OrderNorthwind { OrderID = 2, CustomerID = "CUST2", OrderDate = DateTime.Now.AddDays(-1).ToString() }
        ];

    public static List<OrderNorthwindDetail> OrderDetails =>
        [
            new OrderNorthwindDetail { OrderID = 1, ProductID = 1, Quantity = 10 },
            new OrderNorthwindDetail { OrderID = 1, ProductID = 2, Quantity = 10 },
            new OrderNorthwindDetail { OrderID = 2, ProductID = 2, Quantity = 5 }
        ];
}
