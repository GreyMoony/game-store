using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable CA1861
#pragma warning disable IDE0300

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Genres",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ParentGenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Genres", x => x.Id);
                table.ForeignKey(
                    name: "FK_Genres_Genres_ParentGenreId",
                    column: x => x.ParentGenreId,
                    principalTable: "Genres",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Platforms",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Platforms", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GameGenres",
            columns: table => new
            {
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GenreId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GameGenres", x => new { x.GameId, x.GenreId });
                table.ForeignKey(
                    name: "FK_GameGenres_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GameGenres_Genres_GenreId",
                    column: x => x.GenreId,
                    principalTable: "Genres",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GameGenres_Genres_GenreId1",
                    column: x => x.GenreId1,
                    principalTable: "Genres",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "GamePlatforms",
            columns: table => new
            {
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PlatformId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GamePlatforms", x => new { x.GameId, x.PlatformId });
                table.ForeignKey(
                    name: "FK_GamePlatforms_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GamePlatforms_Platforms_PlatformId",
                    column: x => x.PlatformId,
                    principalTable: "Platforms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GamePlatforms_Platforms_PlatformId1",
                    column: x => x.PlatformId1,
                    principalTable: "Platforms",
                    principalColumn: "Id");
            });

        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Name", "ParentGenreId" },
            values: new object[,]
            {
                { new Guid("03468375-68c6-4f92-9308-6eb2aa7e7326"), "RPG", null },
                { new Guid("2cfc44f6-15c6-4aac-8f4e-ec2963b06ff1"), "Sports", null },
                { new Guid("3e73032c-05f4-49d1-985f-6515af85f71b"), "Strategy", null },
                { new Guid("9199e424-d3fe-4f00-8b20-e73ebaabd14f"), "Action", null },
                { new Guid("ca98c625-fdcf-4cb0-a4be-ac737d7b108e"), "Puzzle & Skill", null },
            });

        migrationBuilder.InsertData(
            table: "Platforms",
            columns: new[] { "Id", "Type" },
            values: new object[,]
            {
                { new Guid("0b5a9586-69b5-408d-9e7d-cb455a7d7750"), "Desktop" },
                { new Guid("3dac5ec8-be4d-4dfe-bd6d-d2805b55360e"), "Browser" },
                { new Guid("553193a3-566e-4505-9eb2-5510eba25c95"), "Mobile" },
                { new Guid("c9672bd2-61f9-4baf-8cd8-3d7190296e09"), "Console" },
            });

        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Name", "ParentGenreId" },
            values: new object[,]
            {
                { new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf"), "Races", new Guid("2cfc44f6-15c6-4aac-8f4e-ec2963b06ff1") },
                { new Guid("76bf1cc9-8a37-454c-84d6-e8c089e379c7"), "FPS", new Guid("9199e424-d3fe-4f00-8b20-e73ebaabd14f") },
                { new Guid("865685b9-eaa6-4ea9-8730-9cc62acd6e57"), "RTS", new Guid("3e73032c-05f4-49d1-985f-6515af85f71b") },
                { new Guid("99e1e8f2-b9c1-4be4-9cea-d250a4904b8b"), "TPS", new Guid("9199e424-d3fe-4f00-8b20-e73ebaabd14f") },
                { new Guid("ccb60df7-c83b-4d51-a403-a907e51d8929"), "TBS", new Guid("3e73032c-05f4-49d1-985f-6515af85f71b") },
                { new Guid("f9394328-c6ab-4844-8904-f5e85989dd6b"), "Adventure", new Guid("9199e424-d3fe-4f00-8b20-e73ebaabd14f") },
                { new Guid("1e7976f5-07c9-47ad-b3a2-7f663a95d37c"), "Arcade", new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf") },
                { new Guid("795f05d2-0a61-4f22-997d-ad30c29f5366"), "Off-road", new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf") },
                { new Guid("80d5f5a0-57c8-4a3d-93b1-62412e75fbff"), "Rally", new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf") },
                { new Guid("bc8167de-4cde-47dd-8454-5dae6fd87e76"), "Formula", new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf") },
            });

        migrationBuilder.CreateIndex(
            name: "IX_GameGenres_GenreId",
            table: "GameGenres",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "IX_GameGenres_GenreId1",
            table: "GameGenres",
            column: "GenreId1");

        migrationBuilder.CreateIndex(
            name: "IX_GamePlatforms_PlatformId",
            table: "GamePlatforms",
            column: "PlatformId");

        migrationBuilder.CreateIndex(
            name: "IX_GamePlatforms_PlatformId1",
            table: "GamePlatforms",
            column: "PlatformId1");

        migrationBuilder.CreateIndex(
            name: "IX_Games_Key",
            table: "Games",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genres_Name",
            table: "Genres",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genres_ParentGenreId",
            table: "Genres",
            column: "ParentGenreId");

        migrationBuilder.CreateIndex(
            name: "IX_Platforms_Type",
            table: "Platforms",
            column: "Type",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GameGenres");

        migrationBuilder.DropTable(
            name: "GamePlatforms");

        migrationBuilder.DropTable(
            name: "Genres");

        migrationBuilder.DropTable(
            name: "Games");

        migrationBuilder.DropTable(
            name: "Platforms");
    }
}
