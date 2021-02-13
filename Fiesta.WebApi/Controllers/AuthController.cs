using Fiesta.Application.Auth;
using Fiesta.Application.Auth.CommonDtos;
using Fiesta.Application.Auth.GoogleLogin;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IAuthService _authService;

        public AuthController(JwtOptions jwtOptions, IAuthService authService)
        {
            _jwtOptions = jwtOptions;
            _authService = authService;
        }

        [HttpGet("google-code-callback")]
        public async Task<ActionResult<AuthResponse>> GoogleCodeCallback(string code, CancellationToken ct)
        {
            var response = await Mediator.Send(new GoogleLoginCommand { Code = code }, ct);
            Response.Cookies.Append(Cookie.RefreshToken, response.RefreshToken, GetRefreshTokenCookieOptions());
            return Ok(response);
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            var (accessToken, newRefreshToken) = await _authService.RefreshJwt(refreshToken, cancellationToken);

            Response.Cookies.Append(Cookie.RefreshToken, newRefreshToken, GetRefreshTokenCookieOptions());
            return Ok(new AuthResponse { AccessToken = accessToken });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            await _authService.Logout(refreshToken, cancellationToken);

            Response.Cookies.Append(Cookie.RefreshToken, string.Empty, GetRefreshTokenCookieOptions(TimeSpan.FromSeconds(0)));
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(EmailPasswordRequest request, CancellationToken cancellationToken)
        {
            var (accessToken, refreshToken) = await _authService.Login(request.Email, request.Password, cancellationToken);
            Response.Cookies.Append(Cookie.RefreshToken, refreshToken, GetRefreshTokenCookieOptions());
            return Ok(new AuthResponse { AccessToken = accessToken });
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterWithEmailAndPassword(RegisterWithEmailAndPassword.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpGet("cloudinary-signature")]
        public async Task<ActionResult<GetCloudinarySignature.Response>> GetCloudinarySignature([FromQuery] GetCloudinarySignature.Query request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        private CookieOptions GetRefreshTokenCookieOptions(TimeSpan? maxAge = null)
            => new CookieOptions
            {
                MaxAge = maxAge ?? _jwtOptions.RefreshTokenLifeTime,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Path = "/",
            };
    }

    public class EmailPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
