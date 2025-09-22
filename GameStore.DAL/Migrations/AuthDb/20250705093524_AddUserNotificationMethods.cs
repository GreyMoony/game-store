using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations.AuthDb;
#pragma warning disable CA1861, IDE0300
/// <inheritdoc />
public partial class AddUserNotificationMethods : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserNotificationMethods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Method = table.Column<string>(type: "nvarchar(450)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserNotificationMethods", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserNotificationMethods_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserNotificationMethods_UserId_Method",
            table: "UserNotificationMethods",
            columns: new[] { "UserId", "Method" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserNotificationMethods");
    }
}
#pragma warning restore CA1861, IDE0300
