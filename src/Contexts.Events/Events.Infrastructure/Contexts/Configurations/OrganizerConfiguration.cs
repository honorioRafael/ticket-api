using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.Contexts.Configurations;

public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
{
    public void Configure(EntityTypeBuilder<Organizer> entity)
    {
        entity.ToTable("organizers");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .IsRequired()
            .HasMaxLength(255);
        entity.Property(e => e.Password)
            .HasConversion(
                pass => pass.Value,
                value => new Password(value))
            .IsRequired()
            .HasMaxLength(255);
        entity.HasIndex(e => e.Email).IsUnique();
    }
}
