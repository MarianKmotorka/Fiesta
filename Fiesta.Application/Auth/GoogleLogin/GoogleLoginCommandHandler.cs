using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth.CommonDtos;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Domain.Entities.Users;
using MediatR;

namespace Fiesta.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponse>
    {
        private readonly GoogleOAuthOptions _oAuthOptions;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        private readonly HttpClient _httpClient;

        public GoogleLoginCommandHandler(GoogleOAuthOptions oAuthOptions, IHttpClientFactory clientFactory, IAuthService authService, IMediator mediator)
        {
            _oAuthOptions = oAuthOptions;
            _authService = authService;
            _mediator = mediator;
            _httpClient = clientFactory.CreateClient();
        }

        public async Task<AuthResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var googleUser = await GetGoogleUser(request.Code);

            var (accessToken, refreshToken, authUserCreated, userId) = await _authService.LoginOrRegister(googleUser, cancellationToken);

            if (authUserCreated)
                await _mediator.Publish(new AuthUserCreatedEvent(userId, googleUser.Email)
                {
                    FirstName = googleUser.GivenName,
                    LastName = googleUser.FamilyName,
                    PictureUrl = googleUser.PictureUrl
                });

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<GoogleUserInfoModel> GetGoogleUser(string code)
        {
            var googleRequest = new
            {
                code,
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

            if (!response.IsSuccessStatusCode)
                throw new BadRequestException("Google service is unavailable");

            return await response.Content.ReadAsAsync<GoogleUserInfoModel>();
        }
    }
}
