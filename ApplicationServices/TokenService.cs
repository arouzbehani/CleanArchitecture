using DomainCore.Entities;
using DomainCore.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ApplicationServices.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly string _secretKey;
        private readonly ISecretRepository _secretRepository;

      

        public TokenService(string secretKey, JwtSettings jwtSettings, ISecretRepository secretRepository)
        {
            _secretKey = secretKey;
            _jwtSettings = jwtSettings;
            _secretRepository = secretRepository;
        }

        public string GenerateAuthenticationToken(string userId, string email)
        {

            string secretKey = _secretRepository.GetSecret("Authentication").GetAwaiter().GetResult();
            var key = Encoding.ASCII.GetBytes(secretKey);

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

        public string GenerateDocumentAccessToken(int documentId,int userId)
        {
            string secretKey = _secretRepository.GetSecret("Document").GetAwaiter().GetResult();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("documentId", documentId.ToString())
                ,new Claim("userId", userId.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(8), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public int GetUserIdByToken(string token)
        {

            // Decode the token and get the user claims
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
            {
                return 0;
            }

            // Extract the userId claim
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return 0;
            }

            var userId = int.Parse(userIdClaim.Value);
            return userId;
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
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return null;
            }
        }



        public int? ValidateDocumentAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string secretKey = _secretRepository.GetSecret("Document").GetAwaiter().GetResult();

            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var documentIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "DocumentId")?.Value;

                return documentIdClaim != null ? int.Parse(documentIdClaim) : (int?)null;
            }
            catch
            {
                return null;
            }
        }

    }

}

