using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public partial class AddedIdToOrderGames : Migration
{
    private static readonly string[] Columns = ["OrderId", "ProductId"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_OrderGames",
            table: "OrderGames");

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "OrderGames",
            type: "uniqueidentifier",
            nullable: false,
            defaultValueSql: "NEWID()");

        migrationBuilder.AddPrimaryKey(
            name: "PK_OrderGames",
            table: "OrderGames",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_OrderGames_OrderId_ProductId",
            table: "OrderGames",
            columns: Columns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_OrderGames",
            table: "OrderGames");

        migrationBuilder.DropIndex(
            name: "IX_OrderGames_OrderId_ProductId",
            table: "OrderGames");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "OrderGames");

        migrationBuilder.AddPrimaryKey(
            name: "PK_OrderGames",
            table: "OrderGames",
            columns: Columns);
    }
}
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

