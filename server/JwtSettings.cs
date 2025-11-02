namespace AIChat1
{
    public sealed class JwtSettings
    {
        public string SecretKey { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpMinutes { get; set; } = 60;
    }

}
