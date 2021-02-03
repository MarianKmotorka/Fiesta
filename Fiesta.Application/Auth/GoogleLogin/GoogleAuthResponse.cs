﻿using Newtonsoft.Json;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}