using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryRider> DeliveryRiders { get; set; }
        public DbSet<RiderLocation> RiderLocations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AdminSettings> AdminSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeliveryRider>()
                .HasOne(r => r.RiderLocation)
                .WithOne(l => l.DeliveryRider)
                .HasForeignKey<RiderLocation>(l => l.DeliveryRiderId);

            modelBuilder.Entity<AdminSettings>()
             .HasOne(s => s.User)
             .WithOne()
             .HasForeignKey<AdminSettings>(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AdminSettings>()
                .HasIndex(s => s.UserId)
                .IsUnique();
        }
    }
}