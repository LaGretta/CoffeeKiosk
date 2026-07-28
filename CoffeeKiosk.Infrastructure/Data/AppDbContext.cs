using CoffeeKiosk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Size> Sizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(n => n.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(n => n.Email).IsUnique();

        modelBuilder.Entity<Order>().Property(n => n.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(n => n.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(n => n.BasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Size>().Property(n => n.PriceModifier).HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .HasMany(n => n.Orders)
            .WithOne(n => n.CreatedByUser)
            .HasForeignKey(n => n.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(n => n.Items)
            .WithOne(n => n.Order)
            .HasForeignKey(n => n.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(n => n.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(n => n.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(n => n.Size)
            .WithMany(s => s.OrderItems)
            .HasForeignKey(n => n.SizeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}