using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class FixGameEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Games_Publishers_PublisherId",
            table: "Games");

        migrationBuilder.AlterColumn<Guid>(
            name: "PublisherId",
            table: "Games",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Games",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddForeignKey(
            name: "FK_Games_Publishers_PublisherId",
            table: "Games",
            column: "PublisherId",
            principalTable: "Publishers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Games_Publishers_PublisherId",
            table: "Games");

        migrationBuilder.AlterColumn<Guid>(
            name: "PublisherId",
            table: "Games",
            type: "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Games",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: string.Empty,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Games_Publishers_PublisherId",
            table: "Games",
            column: "PublisherId",
            principalTable: "Publishers",
            principalColumn: "Id");
    }
}
