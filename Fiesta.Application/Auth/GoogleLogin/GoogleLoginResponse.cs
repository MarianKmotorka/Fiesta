using System.Text.Json.Serialization;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleLoginResponse
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; } // Will be set as  http only cookie
    }
}
