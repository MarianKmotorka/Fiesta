namespace Fiesta.Domain.Entities.Notifications
{
    public enum NotificationType
    {
        None = 0,
        EventInvitationReply = 1,
        EventInvitationCreated = 2,
        EventAttendeeRemoved = 3,
        EventAttendeeLeft = 4,
        EventJoinRequestCreated = 5,
        EventJoinRequestReply = 6,
        FriendRequestReply = 7,
        FriendRemoved = 8,
        NewUserWelcome = 9
    }
}
