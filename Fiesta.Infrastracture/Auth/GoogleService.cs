using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Infrastracture.Auth.Models;

namespace Fiesta.Infrastracture.Auth
{
    internal class GoogleService : IGoogleService
    {
        private readonly GoogleOAuthOptions _authOptions;
        private readonly HttpClient _client;

        public GoogleService(GoogleOAuthOptions authOptions, IHttpClientFactory clientFactory)
        {
            _authOptions = authOptions;
            _client = clientFactory.CreateClient();
        }

        public async Task<Result<GoogleUserInfoModel>> GetUserInfoModel(string code, CancellationToken cancellationToken)
        {
            var request = new
            {
                code,
                client_id = _authOptions.GoogleClientId,
                client_secret = _authOptions.GoogleClientSecret,
                grant_type = "authorization_code",
                redirect_uri = _authOptions.ClientRedirectUri
            };

            var response = await _client.PostAsJsonAsync(_authOptions.TokenEndpoint, request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return Result<GoogleUserInfoModel>.Failure(ErrorCodes.InvalidCode);

            var authResponse = await response.Content.ReadAsAsync<GoogleAuthResponse>();

            var userInfoRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                Headers = { { HttpRequestHeader.Authorization.ToString(), $"{authResponse.TokenType} {authResponse.AccessToken}" } },
                RequestUri = new Uri(_authOptions.UserInfoEndpoint)
            };

            response = await _client.SendAsync(userInfoRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Result<GoogleUserInfoModel>.Failure("Google service is unavailable");

            var model = await response.Content.ReadAsAsync<GoogleUserInfoModel>(cancellationToken);
            return Result.Success(model);
        }
    }
}
