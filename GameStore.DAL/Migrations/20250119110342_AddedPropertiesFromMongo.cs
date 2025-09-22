using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class AddedPropertiesFromMongo : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Address",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "City",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ContactName",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ContactTitle",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Country",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Fax",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Phone",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PostalCode",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Region",
            table: "Publishers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "SupplierID",
            table: "Publishers",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "EmployeeID",
            table: "Orders",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "Freight",
            table: "Orders",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "OrderID",
            table: "Orders",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RequiredDate",
            table: "Orders",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipAddress",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipCity",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipCountry",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipName",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipPostalCode",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ShipRegion",
            table: "Orders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ShipVia",
            table: "Orders",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "ShippedDate",
            table: "Orders",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CategoryID",
            table: "Genres",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Genres",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Picture",
            table: "Genres",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "Discontinued",
            table: "Games",
            type: "bit",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ProductID",
            table: "Games",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "QuantityPerUnit",
            table: "Games",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ReorderLevel",
            table: "Games",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UnitsOnOrder",
            table: "Games",
            type: "int",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Address",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "City",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "ContactName",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "ContactTitle",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "Country",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "Fax",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "Phone",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "PostalCode",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "Region",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "SupplierID",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "EmployeeID",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "Freight",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "OrderID",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "RequiredDate",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipAddress",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipCity",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipCountry",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipName",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipPostalCode",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipRegion",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShipVia",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ShippedDate",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "CategoryID",
            table: "Genres");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "Genres");

        migrationBuilder.DropColumn(
            name: "Picture",
            table: "Genres");

        migrationBuilder.DropColumn(
            name: "Discontinued",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "ProductID",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "QuantityPerUnit",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "ReorderLevel",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "UnitsOnOrder",
            table: "Games");
    }
}
