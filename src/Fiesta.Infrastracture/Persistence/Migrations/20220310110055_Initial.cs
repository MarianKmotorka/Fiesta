using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fiesta.Infrastracture.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fiesta");

            migrationBuilder.CreateTable(
                name: "AuthRole",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthUser",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GoogleEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthProvider = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiestaUser",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiestaUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AuthRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "fiesta",
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AuthUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "fiesta",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AuthUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "fiesta",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AuthUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserAuthRole",
                schema: "fiesta",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserAuthRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AuthUserAuthRole_AuthRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "fiesta",
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthUserAuthRole_AuthUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "AuthUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessibilityType = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_FiestaUser_OrganizerId",
                        column: x => x.OrganizerId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                schema: "fiesta",
                columns: table => new
                {
                    FromId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ToId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => new { x.FromId, x.ToId });
                    table.ForeignKey(
                        name: "FK_FriendRequests_FiestaUser_FromId",
                        column: x => x.FromId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendRequests_FiestaUser_ToId",
                        column: x => x.ToId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "fiesta",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", maxLength: 36, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_FiestaUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFriends",
                schema: "fiesta",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    FriendId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => new { x.UserId, x.FriendId });
                    table.ForeignKey(
                        name: "FK_UserFriends_FiestaUser_FriendId",
                        column: x => x.FriendId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFriends_FiestaUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendees",
                schema: "fiesta",
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
                        principalSchema: "fiesta",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttendees_FiestaUser_AttendeeId",
                        column: x => x.AttendeeId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventComment",
                schema: "fiesta",
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
                        principalSchema: "fiesta",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventComment_EventComment_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "fiesta",
                        principalTable: "EventComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventComment_FiestaUser_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventInvitations",
                schema: "fiesta",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InviterId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    InviteeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitations", x => new { x.EventId, x.InviterId, x.InviteeId });
                    table.ForeignKey(
                        name: "FK_EventInvitations_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "fiesta",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventInvitations_FiestaUser_InviteeId",
                        column: x => x.InviteeId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventInvitations_FiestaUser_InviterId",
                        column: x => x.InviterId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventJoinRequests",
                schema: "fiesta",
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
                        principalSchema: "fiesta",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventJoinRequests_FiestaUser_InterestedUserId",
                        column: x => x.InterestedUserId,
                        principalSchema: "fiesta",
                        principalTable: "FiestaUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationObject",
                schema: "fiesta",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Premise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdministrativeAreaLevel1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdministrativeAreaLevel2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCodeNormalized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleMapsUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationObject", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_LocationObject_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "fiesta",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "fiesta",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "fiesta",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "fiesta",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "fiesta",
                table: "AuthRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "fiesta",
                table: "AuthUser",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUser_Email",
                schema: "fiesta",
                table: "AuthUser",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "fiesta",
                table: "AuthUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserAuthRole_RoleId",
                schema: "fiesta",
                table: "AuthUserAuthRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerId",
                schema: "fiesta",
                table: "Event",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_AttendeeId",
                schema: "fiesta",
                table: "EventAttendees",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_EventId",
                schema: "fiesta",
                table: "EventComment",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_ParentId",
                schema: "fiesta",
                table: "EventComment",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventComment_SenderId",
                schema: "fiesta",
                table: "EventComment",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitations_InviteeId",
                schema: "fiesta",
                table: "EventInvitations",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitations_InviterId",
                schema: "fiesta",
                table: "EventInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_EventJoinRequests_InterestedUserId",
                schema: "fiesta",
                table: "EventJoinRequests",
                column: "InterestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ToId",
                schema: "fiesta",
                table: "FriendRequests",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                schema: "fiesta",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_FriendId",
                schema: "fiesta",
                table: "UserFriends",
                column: "FriendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AuthUserAuthRole",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "EventAttendees",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "EventComment",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "EventInvitations",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "EventJoinRequests",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "FriendRequests",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "LocationObject",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "UserFriends",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AuthRole",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "AuthUser",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "fiesta");

            migrationBuilder.DropTable(
                name: "FiestaUser",
                schema: "fiesta");
        }
    }
}
