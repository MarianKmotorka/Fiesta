using Fiesta.Application.Features.Auth.CommonDtos;

namespace TestBase.Assets
{
    public static class GoogleAssets
    {
        public static GoogleUserInfoModel JohnyUserInfoModel
        {
            get => new GoogleUserInfoModel
            {
                Email = "johny@gmail.com",
                GivenName = "John",
                FamilyName = "Dick",
                IsEmailVerified = true,
                Id = "someId",
            };
        }
    }
}
