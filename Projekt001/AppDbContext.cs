using Microsoft.EntityFrameworkCore;
using Projekt001.Entities;

namespace Projekt001.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ContactEntity> ContactMessages { get; set; }

    }
}