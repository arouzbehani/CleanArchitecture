public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiresInHours { get; set; }
}