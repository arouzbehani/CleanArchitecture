using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly string _secretKey;

    public TokenService(string secretKey, JwtSettings jwtSettings)
    {
        _secretKey = secretKey;
        _jwtSettings = jwtSettings;
    }

    public string GenerateToken(string userId,string email)
    {
        
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),new Claim(ClaimTypes.Email, email)
            }),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    public ClaimsPrincipal GetPrincipalFromToken(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_secretKey);
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, // You might want to validate the token expiration here
    };

    try
    {
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        return principal;
    }
    catch(Exception exp)
    {
        Console.WriteLine(exp.Message);
        return null;
    }
}

}
