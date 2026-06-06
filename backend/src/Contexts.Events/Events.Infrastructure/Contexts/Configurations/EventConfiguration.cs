using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.Contexts.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.ToTable("events");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.OwnsOne(e => e.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("StartsAt").IsRequired();
            period.Property(p => p.End).HasColumnName("EndsAt").IsRequired();
        });
        entity.Property(e => e.VenueId).IsRequired();

        entity.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Navigation(e => e.TicketTypes)
            .HasField("_ticketTypes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
