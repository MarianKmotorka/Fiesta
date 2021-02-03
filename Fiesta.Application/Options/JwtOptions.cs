using System;

namespace Fiesta.Application.Options
{
    public class JwtOptions
    {
        public TimeSpan TokenLifeTime { get; set; }

        public TimeSpan RefreshTokenLifeTime { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }
    }
}
