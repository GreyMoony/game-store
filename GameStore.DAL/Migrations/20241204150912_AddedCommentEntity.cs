using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DAL.Migrations;

/// <inheritdoc />
public partial class AddedCommentEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Comments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Comments_Comments_ParentCommentId",
                    column: x => x.ParentCommentId,
                    principalTable: "Comments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Comments_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Comments_GameId",
            table: "Comments",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_Comments_ParentCommentId",
            table: "Comments",
            column: "ParentCommentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Comments");
    }
}
