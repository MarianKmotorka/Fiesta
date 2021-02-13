using System.Text.Json.Serialization;

namespace Fiesta.Application.Features.Auth.CommonDtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
