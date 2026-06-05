using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.Infrastructure.Contexts.Configurations;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> entity)
    {
        entity.ToTable("ticket_types", t => t.ExcludeFromMigrations());
        entity.HasKey(e => e.Id);
        entity.Property(e => e.EventId).IsRequired();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
        entity.Property(e => e.TotalQuantity).IsRequired();
        entity.Property(e => e.AvailableQuantity).IsRequired();
    }
}
