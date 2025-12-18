using Eleve_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure
{
    public class EleveDbContext : DbContext
    {
        public EleveDbContext(DbContextOptions<EleveDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishListItem> WishItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
