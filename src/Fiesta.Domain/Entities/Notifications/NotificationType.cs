namespace Fiesta.Domain.Entities.Notifications
{
    public enum NotificationType
    {
        None = 0,
        EventInvitationReply = 1,
        EventInvitationCreated = 2,
        EventAttendeeRemoved = 3,
        EventAttendeeLeft = 4
    }
}
