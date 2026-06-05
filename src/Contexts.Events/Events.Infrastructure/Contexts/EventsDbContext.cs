using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Contexts;

public class EventsDbContext : DbContext
{
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Organizer> Organizers => Set<Organizer>();

    public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organizer>(entity =>
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
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("venues");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Capacity).IsRequired();
        });

        modelBuilder.Entity<Event>(entity =>
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
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.ToTable("ticket_types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.TotalQuantity).IsRequired();
            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property(e => e.EventId).IsRequired();

            entity.HasOne<Event>()
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Event>()
            .Navigation(e => e.TicketTypes)
            .HasField("_ticketTypes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
