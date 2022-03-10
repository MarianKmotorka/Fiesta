﻿// <auto-generated />
using System;
using Fiesta.Infrastracture.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fiesta.Infrastracture.Persistence.Migrations
{
    [DbContext(typeof(FiestaDbContext))]
    partial class FiestaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("fiesta")
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.Event", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("AccessibilityType")
                        .HasColumnType("int");

                    b.Property<string>("BannerUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizerId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("OrganizerId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventAttendee", b =>
                {
                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("AttendeeId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("EventId", "AttendeeId");

                    b.HasIndex("AttendeeId");

                    b.ToTable("EventAttendees");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventComment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("IsEdited")
                        .HasColumnType("bit");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("ParentId");

                    b.HasIndex("SenderId");

                    b.ToTable("EventComment");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventInvitation", b =>
                {
                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("InviterId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("InviteeId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("EventId", "InviterId", "InviteeId");

                    b.HasIndex("InviteeId");

                    b.HasIndex("InviterId");

                    b.ToTable("EventInvitations");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventJoinRequest", b =>
                {
                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("InterestedUserId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("EventId", "InterestedUserId");

                    b.HasIndex("InterestedUserId");

                    b.ToTable("EventJoinRequests");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Notifications.Notification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Model")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Seen")
                        .HasColumnType("bit");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.FiestaUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FiestaUser");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.FriendRequest", b =>
                {
                    b.Property<string>("FromId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("ToId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("RequestedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("FromId", "ToId");

                    b.HasIndex("ToId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.UserFriend", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("FriendId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("UserId", "FriendId");

                    b.HasIndex("FriendId");

                    b.ToTable("UserFriends");
                });

            modelBuilder.Entity("Fiesta.Infrastracture.Auth.AuthRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AuthRole");
                });

            modelBuilder.Entity("Fiesta.Infrastracture.Auth.AuthUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int>("AuthProvider")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("GoogleEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("NormalizedEmail")
                        .IsUnique()
                        .HasDatabaseName("EmailIndex")
                        .HasFilter("[NormalizedEmail] IS NOT NULL");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AuthUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AuthUserAuthRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.Event", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Organizer")
                        .WithMany("OrganizedEvents")
                        .HasForeignKey("OrganizerId");

                    b.OwnsOne("Fiesta.Domain.Entities.LocationObject", "Location", b1 =>
                        {
                            b1.Property<string>("EventId")
                                .HasColumnType("nvarchar(36)");

                            b1.Property<string>("AdministrativeAreaLevel1")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("AdministrativeAreaLevel2")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("GoogleMapsUrl")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PostalCodeNormalized")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Premise")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("State")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("StreetNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EventId");

                            b1.ToTable("LocationObject");

                            b1.WithOwner()
                                .HasForeignKey("EventId");
                        });

                    b.Navigation("Location");

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventAttendee", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Attendee")
                        .WithMany("AttendedEvents")
                        .HasForeignKey("AttendeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Events.Event", "Event")
                        .WithMany("Attendees")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attendee");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventComment", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Events.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Events.EventComment", "Parent")
                        .WithMany("Replies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Parent");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventInvitation", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Events.Event", "Event")
                        .WithMany("Invitations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Invitee")
                        .WithMany("RecievedEventInvitations")
                        .HasForeignKey("InviteeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Inviter")
                        .WithMany("SentEventInvitations")
                        .HasForeignKey("InviterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Invitee");

                    b.Navigation("Inviter");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventJoinRequest", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Events.Event", "Event")
                        .WithMany("JoinRequests")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "InterestedUser")
                        .WithMany("SentEventJoinRequests")
                        .HasForeignKey("InterestedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("InterestedUser");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Notifications.Notification", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.FriendRequest", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "From")
                        .WithMany("SentFriendRequests")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "To")
                        .WithMany("RecievedFriendRequests")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("To");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.UserFriend", b =>
                {
                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "Friend")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Fiesta.Domain.Entities.Users.FiestaUser", "User")
                        .WithMany("Friends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friend");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Fiesta.Infrastracture.Auth.AuthRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Fiesta.Infrastracture.Auth.AuthUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Fiesta.Infrastracture.Auth.AuthUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Fiesta.Infrastracture.Auth.AuthRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiesta.Infrastracture.Auth.AuthUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Fiesta.Infrastracture.Auth.AuthUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.Event", b =>
                {
                    b.Navigation("Attendees");

                    b.Navigation("Invitations");

                    b.Navigation("JoinRequests");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Events.EventComment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("Fiesta.Domain.Entities.Users.FiestaUser", b =>
                {
                    b.Navigation("AttendedEvents");

                    b.Navigation("Friends");

                    b.Navigation("OrganizedEvents");

                    b.Navigation("RecievedEventInvitations");

                    b.Navigation("RecievedFriendRequests");

                    b.Navigation("SentEventInvitations");

                    b.Navigation("SentEventJoinRequests");

                    b.Navigation("SentFriendRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
