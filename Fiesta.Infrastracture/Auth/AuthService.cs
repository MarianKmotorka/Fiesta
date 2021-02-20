using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Exceptions;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Options;
using Fiesta.Infrastracture.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Fiesta.Infrastracture.Auth
{
    public partial class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly FiestaDbContext _db;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<AuthUser> _userManager;

        public AuthService(JwtOptions jwtOptions, FiestaDbContext db, TokenValidationParameters tokenValidationParameters,
            UserManager<AuthUser> userManager)
        {
            _db = db;
            _jwtOptions = jwtOptions;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
        }

        public async Task<string> GetEmailVerificationCode(string emailAddress, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailAlreadyVerified);

            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                throw new BadRequestException(ErrorCodes.InvalidAuthProvider);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(x => x.Description));
        }

        public async Task<string> GetResetPasswordToken(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                throw new BadRequestException(ErrorCodes.InvalidAuthProvider);

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task CheckEmailVerificationCode(string emailAddress, string code, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCodes.EmailAlreadyVerified);

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
                throw new BadRequestException(ErrorCodes.InvalidCode);
        }

        public async Task ChangePassword(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new BadRequestException(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                throw new BadRequestException(ErrorCodes.InvalidAuthProvider);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
                throw new BadRequestException(ErrorCodes.InvalidPassword);
        }

        public async Task AddPassword(string userId, string password, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(userId) ?? throw new BadRequestException($"User with id {userId} not found.");

            var result = await _userManager.AddPasswordAsync(user, password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(x => x.Description));

            user.AuthProvider |= AuthProviderEnum.EmailAndPassword;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<FiestaRoleEnum> GetRole(string id)
        {
            return (await _db.Users.FindAsync(id)).Role;
        }

        public async Task<AuthProviderEnum> GetAuthProvider(string id)
        {
            return (await _db.Users.FindAsync(id)).AuthProvider;
        }
    }
}
