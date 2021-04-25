using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class EventComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventComment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventComment_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventComment_EventComment_ParentId",
                        column: x => x.ParentId,
                        principalTable: "EventComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventComment_FiestaUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_EventId",
                table: "EventComment",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_ParentId",
                table: "EventComment",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_SenderId",
                table: "EventComment",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventComment");
        }
    }
}
