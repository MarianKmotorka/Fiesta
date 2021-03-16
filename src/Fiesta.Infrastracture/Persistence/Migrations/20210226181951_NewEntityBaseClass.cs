using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class NewEntityBaseClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "FiestaUser");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "FiestaUser");

            migrationBuilder.DropColumn(
                name: "LastModifiedById",
                table: "FiestaUser");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "FiestaUser",
                newName: "CreatedOnUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "FiestaUser",
                newName: "Created");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "FiestaUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "FiestaUser",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedById",
                table: "FiestaUser",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
