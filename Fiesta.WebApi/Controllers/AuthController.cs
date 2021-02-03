using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth;
using Fiesta.Application.Auth.GoogleLogin;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<GoogleLoginResponse>> GoogleCodeCallback(string code, CancellationToken ct)
        {
            var response = await Mediator.Send(new GoogleLoginCommand { Code = code }, ct);
            Response.Cookies.Append(Cookie.RefreshToken, response.RefreshToken, new CookieOptions
            {
                MaxAge = _jwtOptions.RefreshTokenLifeTime,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Path = "/",
            });
            return Ok(response);
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            var (accessToken, newRefreshToken) = await _authService.RefreshJwt(refreshToken, cancellationToken);
            Response.Cookies.Append(Cookie.RefreshToken, newRefreshToken, new CookieOptions
            {
                MaxAge = _jwtOptions.RefreshTokenLifeTime,
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Path = "/",
            });
            return Ok(new { AccessToken = accessToken });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            await _authService.Logout(refreshToken, cancellationToken);
            Response.Cookies.Append(Cookie.RefreshToken, string.Empty, new CookieOptions
            {
                MaxAge = TimeSpan.FromSeconds(0),
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Path = "/",
            });

            return NoContent();
        }

        [Authorize]
        [HttpGet("cloudinary-signature")]
        public async Task<ActionResult<GetCloudinarySignature.Response>> GetCloudinarySignature([FromQuery] GetCloudinarySignature.Query request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
