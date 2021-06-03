namespace Fiesta.Application.Common.Constants
{
    public class CloudinaryPaths
    {
        public const string ProfilePictures = "ProfilePictures";
        public const string Events = "Events";

        public static string ProfilePicture(string userId) => $"{ProfilePictures}/{userId}";

        public static string EventBanner(string eventId) => $"{Events}/{eventId}/banner";
    }
}
