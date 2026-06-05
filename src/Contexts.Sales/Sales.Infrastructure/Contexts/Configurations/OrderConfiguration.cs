using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Contexts.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        entity.ToTable("orders");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CustomerId).IsRequired();
        entity.Property(e => e.PlacedAt).IsRequired();
        entity.Property(e => e.TotalAmount).IsRequired().HasPrecision(18, 2);
        entity.Property(e => e.Status).IsRequired().HasConversion<string>();

        entity.HasMany(e => e.OrderItems)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Metadata.FindNavigation(nameof(Order.OrderItems))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
