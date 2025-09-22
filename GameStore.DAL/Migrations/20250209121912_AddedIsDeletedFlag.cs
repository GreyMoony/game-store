using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class AddedIsDeletedFlag : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Publishers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Platforms",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Genres",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Games",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Publishers");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Platforms");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Genres");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Games");
    }
}
