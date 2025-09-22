using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class AddedViewCountAndDateOfCreationToGame : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Games",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ViewCount",
            table: "Games",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "ViewCount",
            table: "Games");
    }
}
