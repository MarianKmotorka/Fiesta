﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestBase.Helpers
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, string requestUri, object content)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(client.BaseAddress + requestUri.Trim('/')),
                Method = new HttpMethod("PATCH"),
                Content = jsonContent
            };

            return client.SendAsync(request);
        }
    }
}
