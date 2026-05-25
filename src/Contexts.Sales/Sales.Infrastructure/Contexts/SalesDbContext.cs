using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;

namespace Sales.Infrastructure.Contexts;

public class SalesDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Event> Events => Set<Event>();

    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("sales");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Document).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Document).IsUnique();
        });

        modelBuilder.Entity<Order>(entity =>
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
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.TicketTypeId).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Quantity).IsRequired();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.TicketTypeId).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderItemId).IsRequired();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.Method).IsRequired().HasConversion<string>();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.PaidAt);
        });

        // Tabelas externas do contexto de Events mapeadas dentro do DbContext de Sales
        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.ToTable("ticket_types", "events", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.TotalQuantity).IsRequired();
            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property<uint>("Version")
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events", "events", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.StartsAt).IsRequired();
            entity.Property(e => e.EndsAt).IsRequired();
        });
    }
}
