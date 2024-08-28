namespace DomainCore.Interfaces
{
    public interface ISecurityService
    {
        string HashPassword(string password, out string salt);
        bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt);
    }
}