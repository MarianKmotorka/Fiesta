using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    public partial class SchemaChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fiesta");

            migrationBuilder.RenameTable(
                name: "UserFriends",
                newName: "UserFriends",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notification",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "LocationObject",
                newName: "LocationObject",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                newName: "FriendRequests",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "FiestaUser",
                newName: "FiestaUser",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "EventJoinRequests",
                newName: "EventJoinRequests",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "EventInvitations",
                newName: "EventInvitations",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "EventComment",
                newName: "EventComment",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "EventAttendees",
                newName: "EventAttendees",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Event",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AuthUserAuthRole",
                newName: "AuthUserAuthRole",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AuthUser",
                newName: "AuthUser",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AuthRole",
                newName: "AuthRole",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "fiesta");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "fiesta");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserFriends",
                schema: "fiesta",
                newName: "UserFriends");

            migrationBuilder.RenameTable(
                name: "Notification",
                schema: "fiesta",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "LocationObject",
                schema: "fiesta",
                newName: "LocationObject");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                schema: "fiesta",
                newName: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "FiestaUser",
                schema: "fiesta",
                newName: "FiestaUser");

            migrationBuilder.RenameTable(
                name: "EventJoinRequests",
                schema: "fiesta",
                newName: "EventJoinRequests");

            migrationBuilder.RenameTable(
                name: "EventInvitations",
                schema: "fiesta",
                newName: "EventInvitations");

            migrationBuilder.RenameTable(
                name: "EventComment",
                schema: "fiesta",
                newName: "EventComment");

            migrationBuilder.RenameTable(
                name: "EventAttendees",
                schema: "fiesta",
                newName: "EventAttendees");

            migrationBuilder.RenameTable(
                name: "Event",
                schema: "fiesta",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "AuthUserAuthRole",
                schema: "fiesta",
                newName: "AuthUserAuthRole");

            migrationBuilder.RenameTable(
                name: "AuthUser",
                schema: "fiesta",
                newName: "AuthUser");

            migrationBuilder.RenameTable(
                name: "AuthRole",
                schema: "fiesta",
                newName: "AuthRole");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "fiesta",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "fiesta",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "fiesta",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "fiesta",
                newName: "AspNetRoleClaims");
        }
    }
}
