using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814, CA1861, SA1413, IDE0300, IDE0161

namespace GameStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSeedingFromModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentGenreId" },
                values: new object[,]
                {
                    { new Guid("0fc97d07-bfe9-4ad2-be31-5d210956a31f"), "Sports", null },
                    { new Guid("65017b1f-7748-4bf0-a1e1-43a7d576cfce"), "Puzzle & Skill", null },
                    { new Guid("7dd4f1d3-f92e-4f4a-bb36-424682663aef"), "RPG", null },
                    { new Guid("ba11af97-586e-40a2-9715-cdab4de652dd"), "Action", null },
                    { new Guid("e09418e8-d5e9-49c0-9b6d-896217a4e4a2"), "Strategy", null }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("6f048bbd-ab80-4d78-a4a9-63dc838d2e12"), "Mobile" },
                    { new Guid("70c393e6-11dc-47da-b98d-5f67c858ecee"), "Desktop" },
                    { new Guid("d7ebd850-864d-4990-aa64-53e586cd6f10"), "Browser" },
                    { new Guid("ef29b863-2893-46bb-bd91-4019ab36b1b8"), "Console" }
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
                    { new Guid("c5fb4ec1-d9f8-4a03-ac2b-dda7cf6976a5"), "Rally", new Guid("495e095a-a191-4b71-b20f-33abfa30fc70") }
                });
        }
    }
}
