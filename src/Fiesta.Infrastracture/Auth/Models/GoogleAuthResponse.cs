using Newtonsoft.Json;

namespace Fiesta.Infrastracture.Auth.Models
{
    public class GoogleAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
