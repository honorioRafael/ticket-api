using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.Contexts.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> entity)
    {
        entity.ToTable("venues");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
        entity.Property(e => e.Capacity).IsRequired();
    }
}
