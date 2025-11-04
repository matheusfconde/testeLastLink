using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductEvent> ProductEvents { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired();
            b.Property(p => p.Category).IsRequired();
            b.Property(p => p.UnitCost).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<ProductEvent>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.EventType).IsRequired();
            b.Property(e => e.Payload).IsRequired();
            b.Property(e => e.CreatedAt).IsRequired();
        });
    }
}

public class ProductEvent
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
