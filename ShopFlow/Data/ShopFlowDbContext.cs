using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;

namespace ShopFlow.Data;

public class ShopFlowDbContext : DbContext
{
    public ShopFlowDbContext(DbContextOptions<ShopFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.StockQuantity).IsRequired();

            entity.HasData(
                new Product { Id = 1, Name = "Laptop", Price = 1000m, StockQuantity = 10 },
                new Product { Id = 2, Name = "Mouse", Price = 50m, StockQuantity = 20 },
                new Product { Id = 3, Name = "Keyboard", Price = 100m, StockQuantity = 15 }
            );
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.UserId).IsRequired().HasMaxLength(100);
            entity.HasMany(c => c.Items)
                  .WithOne(i => i.Cart)
                  .HasForeignKey(i => i.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.HasOne(ci => ci.Product)
                  .WithMany()
                  .HasForeignKey(ci => ci.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
