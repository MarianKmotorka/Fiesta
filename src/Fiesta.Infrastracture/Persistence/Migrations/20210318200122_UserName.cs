using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class UserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AuthUser");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "FiestaUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AuthUser",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_Email",
                table: "AuthUser",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AuthUser");

            migrationBuilder.DropIndex(
                name: "IX_AuthUser_Email",
                table: "AuthUser");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "FiestaUser");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AuthUser",
                column: "NormalizedEmail");
        }
    }
}
