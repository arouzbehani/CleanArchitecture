// File: Infrastructure/Persistence/AppDbContext.cs
using DomainCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Secret> Secrets { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}
