using DomainCore.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security
{
    public class SecretRepository : ISecretRepository
    {
        private readonly AppDbContext _context;

        public SecretRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetSecret(string domain)
        {
            var secret = await _context.Secrets.FirstOrDefaultAsync(x => x.Domain == domain);
            return secret?.Key;
        }
    }
}
