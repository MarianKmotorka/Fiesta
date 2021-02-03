using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Options;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResponse>
    {
        private readonly GoogleOAuthOptions _oAuthOptions;
        private readonly IAuthService _authService;
        private readonly JwtOptions _jwtOptions;
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;

        public GoogleLoginCommandHandler(IHttpContextAccessor httpContextAccessor, GoogleOAuthOptions oAuthOptions,
            IHttpClientFactory clientFactory, IAuthService authService, JwtOptions jwtOptions)
        {
            _oAuthOptions = oAuthOptions;
            _authService = authService;
            _jwtOptions = jwtOptions;
            _httpClient = clientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<GoogleLoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var googleRequest = new
            {
                code = request.Code,
                client_id = _oAuthOptions.GoogleClientId,
                client_secret = _oAuthOptions.GoogleClientSecret,
                grant_type = "authorization_code",
                redirect_uri = _oAuthOptions.ClientRedirectUri
            };

            var response = await _httpClient.PostAsJsonAsync(_oAuthOptions.TokenEndpoint, googleRequest);
            var authResponse = await response.Content.ReadAsAsync<GoogleAuthResponse>();

            if (!response.IsSuccessStatusCode)
                throw new BadRequestException("Invalid code");

            var userInfoRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                Headers = { { HttpRequestHeader.Authorization.ToString(), $"{authResponse.TokenType} {authResponse.AccessToken}" } },
                RequestUri = new Uri(_oAuthOptions.UserInfoEndpoint)
            };

            response = await _httpClient.SendAsync(userInfoRequest);
            var userInfoResponse = await response.Content.ReadAsAsync<GoogleUserInfoModel>();

            var (accessToken, refreshToken) = await _authService.Login(userInfoResponse, cancellationToken);


            return new GoogleLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
