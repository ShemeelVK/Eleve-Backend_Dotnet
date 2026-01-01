using Eleve_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Infrastructure.Persistence
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
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //configuring address as valueObject
            modelBuilder.Entity<Order>()
                .OwnsOne(o => o.ShippingAddress);

            //configure Order into OrderItems relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId) //linking to the guid ID of Order
                .OnDelete(DeleteBehavior.Cascade);

            //Money Handling
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i=>i.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
