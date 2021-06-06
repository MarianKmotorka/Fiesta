namespace Fiesta.Application.Common.Constants
{
    public class CloudinaryPaths
    {
        private const string ProfilePictures = "ProfilePictures";
        private const string Events = "Events";

        public static string ProfilePicture(string userId) => $"{ProfilePictures}/{userId}";

        public static string EventBanner(string eventId) => $"{Events}/{eventId}/banner";

        public static string EventFolder(string eventId) => $"{Events}/{eventId}";
    }
}
