using System.Text.Json.Serialization;

namespace Fiesta.Application.Auth.CommonDtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
