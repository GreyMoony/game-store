using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class AddGameTranslations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GameTranslations",
            columns: table => new
            {
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LanguageCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GameTranslations", x => new { x.GameId, x.LanguageCode });
                table.ForeignKey(
                    name: "FK_GameTranslations_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql(@"
            CREATE OR ALTER FUNCTION dbo.GetGameLocalized(
                @GameKey NVARCHAR(10), 
                @Lang NVARCHAR(10),
                @IncludeDeleted BIT = 0
            )
            RETURNS TABLE
            AS
            RETURN
            (
                SELECT 
                    g.Id,
                    g.[Key],
                    g.Price,
                    g.UnitInStock,
                    g.Discount,
                    g.PublisherId,
                    g.ViewCount,
                    g.CreatedAt,
                    g.ImageName,
                    COALESCE(t.Name, g.Name) AS Name,
                    COALESCE(t.Description, g.Description) AS Description
                FROM Games g
                LEFT JOIN GameTranslations t
                    ON g.Id = t.GameId AND t.LanguageCode = @Lang
                WHERE g.[Key] = @GameKey
                    AND (@IncludeDeleted = 1 OR g.IsDeleted = 0)
            );
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GameTranslations");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.GetGameLocalized;");
    }
}
