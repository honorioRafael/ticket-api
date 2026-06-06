using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sales.Infrastructure.Contexts.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.ToTable("events", t => t.ExcludeFromMigrations());
        entity.HasKey(e => e.Id);
        entity.OwnsOne(e => e.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("StartsAt").IsRequired();
            period.Property(p => p.End).HasColumnName("EndsAt").IsRequired();
        });
    }
}
