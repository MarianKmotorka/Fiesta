using Fiesta.Application.Common.Constants;
using Fiesta.Domain.Entities.Users;
using Fiesta.Infrastracture.Auth;
using Fiesta.Infrastracture.Persistence;
using System;

namespace TestBase.Assets
{
    public static class UserAssets
    {
        public static (AuthUser authUser, FiestaUser fiestaUser) SeedBasicUser(this FiestaDbContext db, Action<FiestaUser> configureFiestaUser = null,
            Action<AuthUser> configureAuthUser = null)
        {
            var user = new AuthUser("bob@test.com", FiestaRoleEnum.BasicUser, AuthProviderEnum.EmailAndPassword, "Bobby")
            {
                Id = Guid.NewGuid().ToString(),
                PasswordHash = "some_fake_passsword_hash",
            };

            var fiestaUser = FiestaUser.CreateWithId(user.Id, user.Email, user.Nickname);

            if (configureFiestaUser is not null)
                configureFiestaUser(fiestaUser);

            if (configureAuthUser is not null)
                configureAuthUser(user);

            db.AddRangeAsync(user, fiestaUser);
            return (user, fiestaUser);
        }

        public static (AuthUser authUser, FiestaUser fiestaUser) SeedAdmin(this FiestaDbContext db, Action<FiestaUser> configureFiestaUser = null,
            Action<AuthUser> configureAuthUser = null)
        {
            var user = new AuthUser("fred@test.com", FiestaRoleEnum.Admin, AuthProviderEnum.EmailAndPassword, "Freddy")
            {
                Id = Guid.NewGuid().ToString(),
                PasswordHash = "some_fake_passsword_hash",
            };

            var fiestaUser = FiestaUser.CreateWithId(user.Id, user.Email, user.Nickname);

            if (configureFiestaUser is not null)
                configureFiestaUser(fiestaUser);

            if (configureAuthUser is not null)
                configureAuthUser(user);

            db.AddRangeAsync(user, fiestaUser);
            return (user, fiestaUser);
        }
    }
}
