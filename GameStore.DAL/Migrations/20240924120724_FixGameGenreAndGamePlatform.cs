using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814, CA1861, SA1413, IDE0300, IDE0161

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class FixGameGenreAndGamePlatform : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GameGenres_Genres_GenreId1",
            table: "GameGenres");

        migrationBuilder.DropForeignKey(
            name: "FK_GamePlatforms_Platforms_PlatformId1",
            table: "GamePlatforms");

        migrationBuilder.DropIndex(
            name: "IX_GamePlatforms_PlatformId1",
            table: "GamePlatforms");

        migrationBuilder.DropIndex(
            name: "IX_GameGenres_GenreId1",
            table: "GameGenres");

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("03468375-68c6-4f92-9308-6eb2aa7e7326"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("1e7976f5-07c9-47ad-b3a2-7f663a95d37c"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("76bf1cc9-8a37-454c-84d6-e8c089e379c7"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("795f05d2-0a61-4f22-997d-ad30c29f5366"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("80d5f5a0-57c8-4a3d-93b1-62412e75fbff"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("865685b9-eaa6-4ea9-8730-9cc62acd6e57"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("99e1e8f2-b9c1-4be4-9cea-d250a4904b8b"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("bc8167de-4cde-47dd-8454-5dae6fd87e76"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("ca98c625-fdcf-4cb0-a4be-ac737d7b108e"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("ccb60df7-c83b-4d51-a403-a907e51d8929"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("f9394328-c6ab-4844-8904-f5e85989dd6b"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("0b5a9586-69b5-408d-9e7d-cb455a7d7750"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("3dac5ec8-be4d-4dfe-bd6d-d2805b55360e"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("553193a3-566e-4505-9eb2-5510eba25c95"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("c9672bd2-61f9-4baf-8cd8-3d7190296e09"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("3e390490-0c39-4b7c-b3eb-1fcf00ae9eaf"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("3e73032c-05f4-49d1-985f-6515af85f71b"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("9199e424-d3fe-4f00-8b20-e73ebaabd14f"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("2cfc44f6-15c6-4aac-8f4e-ec2963b06ff1"));

        migrationBuilder.DropColumn(
            name: "PlatformId1",
            table: "GamePlatforms");

        migrationBuilder.DropColumn(
            name: "GenreId1",
            table: "GameGenres");

        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Name", "ParentGenreId" },
            values: new object[,]
            {
                { new Guid("0fc97d07-bfe9-4ad2-be31-5d210956a31f"), "Sports", null },
                { new Guid("65017b1f-7748-4bf0-a1e1-43a7d576cfce"), "Puzzle & Skill", null },
                { new Guid("7dd4f1d3-f92e-4f4a-bb36-424682663aef"), "RPG", null },
                { new Guid("ba11af97-586e-40a2-9715-cdab4de652dd"), "Action", null },
                { new Guid("e09418e8-d5e9-49c0-9b6d-896217a4e4a2"), "Strategy", null },
            });

        migrationBuilder.InsertData(
            table: "Platforms",
            columns: new[] { "Id", "Type" },
            values: new object[,]
            {
                { new Guid("6f048bbd-ab80-4d78-a4a9-63dc838d2e12"), "Mobile" },
                { new Guid("70c393e6-11dc-47da-b98d-5f67c858ecee"), "Desktop" },
                { new Guid("d7ebd850-864d-4990-aa64-53e586cd6f10"), "Browser" },
                { new Guid("ef29b863-2893-46bb-bd91-4019ab36b1b8"), "Console" },
            });

        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Name", "ParentGenreId" },
            values: new object[,]
            {
                { new Guid("495e095a-a191-4b71-b20f-33abfa30fc70"), "Races", new Guid("0fc97d07-bfe9-4ad2-be31-5d210956a31f") },
                { new Guid("918c1e22-f319-4c62-9099-9b97aa161608"), "TBS", new Guid("e09418e8-d5e9-49c0-9b6d-896217a4e4a2") },
                { new Guid("9f7b52dc-2c09-4e35-bce7-c8794013d3e1"), "FPS", new Guid("ba11af97-586e-40a2-9715-cdab4de652dd") },
                { new Guid("d192a43c-324e-40dd-b251-46fad07c97c9"), "Adventure", new Guid("ba11af97-586e-40a2-9715-cdab4de652dd") },
                { new Guid("d1b0e087-8736-45ef-944c-987a0649d7e6"), "TPS", new Guid("ba11af97-586e-40a2-9715-cdab4de652dd") },
                { new Guid("e9971717-02aa-4592-a59c-05d2e57cd2e7"), "RTS", new Guid("e09418e8-d5e9-49c0-9b6d-896217a4e4a2") },
                { new Guid("1cb0629d-4eab-4146-b630-315524a41e76"), "Arcade", new Guid("495e095a-a191-4b71-b20f-33abfa30fc70") },
                { new Guid("a24bca53-0d1e-4ad7-a943-ae369e342439"), "Off-road", new Guid("495e095a-a191-4b71-b20f-33abfa30fc70") },
                { new Guid("b36f94d1-c725-4468-b753-1eccf69a0eb8"), "Formula", new Guid("495e095a-a191-4b71-b20f-33abfa30fc70") },
                { new Guid("c5fb4ec1-d9f8-4a03-ac2b-dda7cf6976a5"), "Rally", new Guid("495e095a-a191-4b71-b20f-33abfa30fc70") },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("1cb0629d-4eab-4146-b630-315524a41e76"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("65017b1f-7748-4bf0-a1e1-43a7d576cfce"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("7dd4f1d3-f92e-4f4a-bb36-424682663aef"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("918c1e22-f319-4c62-9099-9b97aa161608"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("9f7b52dc-2c09-4e35-bce7-c8794013d3e1"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("a24bca53-0d1e-4ad7-a943-ae369e342439"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("b36f94d1-c725-4468-b753-1eccf69a0eb8"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("c5fb4ec1-d9f8-4a03-ac2b-dda7cf6976a5"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("d192a43c-324e-40dd-b251-46fad07c97c9"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("d1b0e087-8736-45ef-944c-987a0649d7e6"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("e9971717-02aa-4592-a59c-05d2e57cd2e7"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("6f048bbd-ab80-4d78-a4a9-63dc838d2e12"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("70c393e6-11dc-47da-b98d-5f67c858ecee"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("d7ebd850-864d-4990-aa64-53e586cd6f10"));

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: new Guid("ef29b863-2893-46bb-bd91-4019ab36b1b8"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("495e095a-a191-4b71-b20f-33abfa30fc70"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("ba11af97-586e-40a2-9715-cdab4de652dd"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("e09418e8-d5e9-49c0-9b6d-896217a4e4a2"));

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: new Guid("0fc97d07-bfe9-4ad2-be31-5d210956a31f"));

        migrationBuilder.AddColumn<Guid>(
            name: "PlatformId1",
            table: "GamePlatforms",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "GenreId1",
            table: "GameGenres",
            type: "uniqueidentifier",
            nullable: true);

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
            name: "IX_GamePlatforms_PlatformId1",
            table: "GamePlatforms",
            column: "PlatformId1");

        migrationBuilder.CreateIndex(
            name: "IX_GameGenres_GenreId1",
            table: "GameGenres",
            column: "GenreId1");

        migrationBuilder.AddForeignKey(
            name: "FK_GameGenres_Genres_GenreId1",
            table: "GameGenres",
            column: "GenreId1",
            principalTable: "Genres",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_GamePlatforms_Platforms_PlatformId1",
            table: "GamePlatforms",
            column: "PlatformId1",
            principalTable: "Platforms",
            principalColumn: "Id");
    }
}
