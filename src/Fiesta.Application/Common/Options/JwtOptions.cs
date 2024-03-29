﻿using System;

namespace Fiesta.Application.Common.Options
{
    public class JwtOptions
    {
        public TimeSpan TokenLifeTime { get; set; }

        public TimeSpan RefreshTokenLifeTime { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }
    }
}
