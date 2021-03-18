using Fiesta.Application.Common.Constants;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Common.Models;
using Fiesta.Application.Common.Options;
using Fiesta.Application.Features.Auth.CommonDtos;
using Fiesta.Infrastracture.Helpers;
using Fiesta.Infrastracture.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<Result<string>> GetEmailVerificationCode(string emailAddress, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                return Result<string>.Failure(ErrorCodes.InvalidEmailAddress);

            if (user.EmailConfirmed)
                return Result<string>.Failure(ErrorCodes.EmailAlreadyVerified);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Result.Success(token);
        }

        public async Task<Result> ResetPassword(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Failure(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                return Result.Failure(ErrorCodes.InvalidAuthProvider);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(x => x.Description));

            return Result.Success();
        }

        public async Task<Result<string>> GetResetPasswordToken(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result<string>.Failure(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                return Result<string>.Failure(ErrorCodes.InvalidAuthProvider);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Result.Success(token);
        }

        public async Task<Result> CheckEmailVerificationCode(string emailAddress, string code, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(emailAddress);

            if (user is null)
                return Result.Failure(ErrorCodes.InvalidEmailAddress);

            if (user.EmailConfirmed)
                return Result.Failure(ErrorCodes.EmailAlreadyVerified);

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                return Result.Failure(ErrorCodes.InvalidCode);

            return Result.Success();
        }

        public async Task<Result> ChangePassword(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                return Result.Failure(ErrorCodes.InvalidAuthProvider);

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Result.Failure(ErrorCodes.InvalidPassword);

            return Result.Success();
        }

        public async Task<Result> AddPassword(string userId, string password, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
                return Result.Failure($"User with id {userId} not found.");

            var result = await _userManager.AddPasswordAsync(user, password);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(x => x.Description));

            user.AddEmailAndPasswordAuthProvider();
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> AddGoogleAccount(string userId, GoogleUserInfoModel model, CancellationToken cancellationToken)
        {
            var isEmailUsed = await _db.Users
                .Where(x => x.Id != userId)
                .WhereSomeEmailIs(model.Email)
                .AnyAsync(cancellationToken);

            if (isEmailUsed)
                return Result.Failure(ErrorCodes.EmailAreadyInUse);

            var user = await _db.Users.FindAsync(userId);
            if (user is null)
                return Result.Failure($"User with id {userId} not found.");

            user.AddGoogleAuthProvider(model.Email);

            if (user.Email == user.GoogleEmail && model.IsEmailVerified)
                user.EmailConfirmed = true;

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<FiestaRoleEnum> GetRole(string userId, CancellationToken cancellationToken)
        {
            return (await _db.Users.FindAsync(new[] { userId }, cancellationToken)).Role;
        }

        public async Task<AuthProviderEnum> GetAuthProvider(string userId, CancellationToken cancellationToken)
        {
            return (await _db.Users.FindAsync(new[] { userId }, cancellationToken)).AuthProvider;
        }

        public async Task<string> GetGoogleEmail(string userId, CancellationToken cancellationToken)
        {
            return (await _db.Users.FindAsync(new[] { userId }, cancellationToken)).GoogleEmail;
        }

        public async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
        {
            var emailExists = await _db.Users.AsQueryable().WhereSomeEmailIs(email).AnyAsync(cancellationToken);
            return !emailExists;
        }

        public async Task<Result> DeleteAccountWithPassword(string userId, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure(ErrorCodes.InvalidEmailAddress);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.EmailAndPassword))
                return Result.Failure(ErrorCodes.InvalidAuthProvider);

            var passValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passValid)
                return Result.Failure(ErrorCodes.InvalidPassword);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(x => x.Description));

            var fiestaUser = await _db.FiestaUsers.FindAsync(new[] { userId }, cancellationToken);
            fiestaUser.IsDeleted = true;

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAccountWithGoogle(string userId, GoogleUserInfoModel googleUser, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (!user.AuthProvider.HasFlag(AuthProviderEnum.Google))
                return Result.Failure(ErrorCodes.InvalidAuthProvider);

            if (user.GoogleEmail != googleUser.Email)
                return Result.Failure(ErrorCodes.GoogleAccountNotConnected);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(x => x.Description));

            var fiestaUser = await _db.FiestaUsers.FindAsync(new[] { userId }, cancellationToken);
            fiestaUser.IsDeleted = true;

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<string>> GenerateNickname(string email, CancellationToken cancellationToken)
        {
            if (!email.Contains('@'))
                return Result<string>.Failure(ErrorCodes.InvalidEmailAddress);

            var host = email.Split('@')[0];

            if (host.Length > 4)
                host = host.Remove(4);

            var nickname = "";

            while (true)
            {
                var ticks = DateTime.Now.Ticks.ToString();
                nickname = $"{host}#{ticks.Remove(0, 9 + host.Length)}";

                if (!(await _db.FiestaUsers.AnyAsync(x => x.Nickname == nickname, cancellationToken)))
                    break;
            }

            return Result.Success(nickname);
        }
    }
}
