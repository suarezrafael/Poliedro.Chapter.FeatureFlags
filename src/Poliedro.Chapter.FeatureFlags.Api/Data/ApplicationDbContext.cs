using Microsoft.EntityFrameworkCore;
using Poliedro.Chapter.FeatureFlags.Api.Models;

namespace Poliedro.Chapter.FeatureFlags.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasMany(e => e.Items).WithOne().HasForeignKey(e => e.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Notebook Dell Inspiron",
                Description = "Notebook com processador Intel Core i7, 16GB RAM, 512GB SSD",
                BasePrice = 4500.00m,
                Category = "Eletrônicos",
                StockQuantity = 15,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Product
            {
                Id = 2,
                Name = "Mouse Logitech MX Master",
                Description = "Mouse sem fio ergonômico de alta precisão",
                BasePrice = 450.00m,
                Category = "Periféricos",
                StockQuantity = 50,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Product
            {
                Id = 3,
                Name = "Teclado Mecânico RGB",
                Description = "Teclado mecânico com iluminação RGB e switches blue",
                BasePrice = 350.00m,
                Category = "Periféricos",
                StockQuantity = 30,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Product
            {
                Id = 4,
                Name = "Monitor LG UltraWide 34\"",
                Description = "Monitor ultrawide 34 polegadas, resolução 2K",
                BasePrice = 2800.00m,
                Category = "Eletrônicos",
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Product
            {
                Id = 5,
                Name = "Webcam Logitech C920",
                Description = "Webcam Full HD 1080p com microfone embutido",
                BasePrice = 550.00m,
                Category = "Periféricos",
                StockQuantity = 25,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Product
            {
                Id = 6,
                Name = "Headset HyperX Cloud II",
                Description = "Headset gamer com som surround 7.1",
                BasePrice = 600.00m,
                Category = "Periféricos",
                StockQuantity = 20,
                IsActive = false, // Produto inativo para demonstrar feature flag
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        );
    }
}
