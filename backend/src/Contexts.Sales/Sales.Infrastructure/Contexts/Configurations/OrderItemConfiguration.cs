using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Contexts.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> entity)
    {
        entity.ToTable("order_items");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.OrderId).IsRequired();
        entity.Property(e => e.TicketTypeId).IsRequired();
        entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
        entity.Property(e => e.Quantity).IsRequired();

        entity.Navigation(i => i.Tickets)
            .HasField("_tickets")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
