using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Contexts.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> entity)
    {
        entity.ToTable("tickets");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.OrderItemId).IsRequired();
        entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Status).IsRequired().HasConversion<string>();
        entity.HasIndex(e => e.Code).IsUnique();

        entity.HasOne<OrderItem>()
            .WithMany(i => i.Tickets)
            .HasForeignKey(e => e.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
