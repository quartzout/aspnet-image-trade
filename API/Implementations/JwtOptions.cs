﻿namespace API.Implementations
{
    public class JwtOptions
    {
        public const string SectionName = "JwtOptions";

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpireDays { get; set; } = 0;

    }
}
