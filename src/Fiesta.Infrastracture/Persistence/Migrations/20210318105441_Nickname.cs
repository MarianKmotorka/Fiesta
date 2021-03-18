using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class Nickname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "FiestaUser",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "AuthUser",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiestaUser_Nickname",
                table: "FiestaUser",
                column: "Nickname",
                unique: true,
                filter: "[Nickname] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_Nickname",
                table: "AuthUser",
                column: "Nickname",
                unique: true,
                filter: "[Nickname] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FiestaUser_Nickname",
                table: "FiestaUser");

            migrationBuilder.DropIndex(
                name: "IX_AuthUser_Nickname",
                table: "AuthUser");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "FiestaUser");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "AuthUser");
        }
    }
}
