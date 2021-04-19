using System;

namespace SquashLeague.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}