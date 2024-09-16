
using System.Security.Claims;

namespace DomainCore.Interfaces
{

    public interface ITokenService
    {
        string GenerateAuthenticationToken(string userId, string email);
        string GenerateDocumentAccessToken(int documentId,int userId);
        int? ValidateDocumentAccessToken(string accessToken);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        int GetUserIdByToken(string token);
    }

}
