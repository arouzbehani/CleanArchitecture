
using System;
using System.Security.Cryptography;
using System.Text;
using DomainCore.Interfaces;

namespace Infrastructure.Security
{
    public class SecurityService : ISecurityService
    {
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16]; // 128-bit salt
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, out string salt)
        {
            salt=GenerateSalt();
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{salt}{password}";
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var hashOfEnteredPassword = HashPassword(enteredPassword, out storedSalt);
            return hashOfEnteredPassword == storedHash;
        }
    }
}
