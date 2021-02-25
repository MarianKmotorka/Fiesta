using System;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Features.Auth;
using Fiesta.Application.Features.Auth.CommonDtos;
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

        [HttpPost("google-login")]
        public async Task<ActionResult<AuthResponse>> LoginWithCode(GoogleLogin.Command request, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(request, cancellationToken);
            Response.Cookies.Append(Cookie.RefreshToken, response.RefreshToken, GetRefreshTokenCookieOptions());
            return Ok(response);
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            var result = await _authService.RefreshJwt(refreshToken, cancellationToken);
            if (result.Failed)
                throw new BadRequestException(result.Errors);

            var (accessToken, newRefreshToken) = result.Data;
            Response.Cookies.Append(Cookie.RefreshToken, newRefreshToken, GetRefreshTokenCookieOptions());
            return Ok(new AuthResponse { AccessToken = accessToken });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken)
        {
            Request.Cookies.TryGetValue(Cookie.RefreshToken, out string refreshToken);
            var result = await _authService.Logout(refreshToken, cancellationToken);
            if (result.Failed)
                throw new BadRequestException(result.Errors);

            Response.Cookies.Append(Cookie.RefreshToken, string.Empty, GetRefreshTokenCookieOptions(TimeSpan.FromSeconds(0)));
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(EmailPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.Login(request.Email, request.Password, cancellationToken);
            if (result.Failed)
                throw new BadRequestException(result.Errors);

            var (accessToken, refreshToken) = result.Data;
            Response.Cookies.Append(Cookie.RefreshToken, refreshToken, GetRefreshTokenCookieOptions());
            return Ok(new AuthResponse { AccessToken = accessToken });
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterWithEmailAndPassword(RegisterWithEmailAndPassword.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpGet("cloudinary-signature")]
        public async Task<ActionResult<GetCloudinarySignature.Response>> GetCloudinarySignature([FromQuery] GetCloudinarySignature.Query request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail(EmailVerificationRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.CheckEmailVerificationCode(request.Email, request.Code, cancellationToken);
            if (result.Failed)
                throw new BadRequestException(result.Errors);

            return NoContent();
        }

        [HttpPost("send-verification-email")]
        public async Task<ActionResult> SendVerificationEmail(SendVerificationEmail.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("send-reset-password-email")]
        public async Task<ActionResult> SendResetPasswordEmail(SendResetPasswordEmail.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPassword.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePassword.Command request, CancellationToken cancellationToken)
        {
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("add-password")]
        public async Task<ActionResult> AddPassword(AddPassword.Command request, CancellationToken cancellationToken)
        {
            request.UserId = CurrentUserService.UserId;
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpPost("connect-google-account")]
        public async Task<ActionResult> ConnectGoogleAccount(ConnectGoogleAccount.Command request, CancellationToken cancellationToken)
        {
            request.UserId = CurrentUserService.UserId;
            await Mediator.Send(request, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpDelete("delete-account-with-password")]
        public async Task<ActionResult> DeleteAccountWithPassword(string password, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteAccountWithPassword.Command { UserId = CurrentUserService.UserId, Password = password }, cancellationToken);
            return NoContent();
        }

        [Authorize(nameof(FiestaRoleEnum.BasicUser))]
        [HttpDelete("delete-account-with-google")]
        public async Task<ActionResult> DeleteAccountWithGoogle(string code, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteAccountWithGoogle.Command { UserId = CurrentUserService.UserId, Code = code }, cancellationToken);
            return NoContent();
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

    public class EmailVerificationRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
