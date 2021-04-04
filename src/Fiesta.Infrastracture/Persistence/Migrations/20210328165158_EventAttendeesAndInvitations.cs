using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class EventAttendeesAndInvitations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Event",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventAttendees",
                columns: table => new
                {
                    AttendeeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendees", x => new { x.EventId, x.AttendeeId });
                    table.ForeignKey(
                        name: "FK_EventAttendees_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttendees_FiestaUser_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventInvitations",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InviterId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InviteeId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitations", x => new { x.EventId, x.InviterId, x.InviteeId });
                    table.ForeignKey(
                        name: "FK_EventInvitations_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventInvitations_FiestaUser_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventInvitations_FiestaUser_InviterId",
                        column: x => x.InviterId,
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventJoinRequests",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InterestedUserId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventJoinRequests", x => new { x.EventId, x.InterestedUserId });
                    table.ForeignKey(
                        name: "FK_EventJoinRequests_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventJoinRequests_FiestaUser_InterestedUserId",
                        column: x => x.InterestedUserId,
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_AttendeeId",
                table: "EventAttendees",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitations_InviteeId",
                table: "EventInvitations",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitations_InviterId",
                table: "EventInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_EventJoinRequests_InterestedUserId",
                table: "EventJoinRequests",
                column: "InterestedUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendees");

            migrationBuilder.DropTable(
                name: "EventInvitations");

            migrationBuilder.DropTable(
                name: "EventJoinRequests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Event");
        }
    }
}
