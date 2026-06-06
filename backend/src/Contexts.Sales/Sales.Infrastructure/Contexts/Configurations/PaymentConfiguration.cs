using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Contexts.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> entity)
    {
        entity.ToTable("payments");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.OrderId).IsRequired();
        entity.Property(e => e.Method).IsRequired().HasConversion<string>();
        entity.Property(e => e.Status).IsRequired().HasConversion<string>();
        entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
        entity.Property(e => e.PaidAt);
    }
}
