using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain.Entities;
using Sales.Domain.ValueObjects;

namespace Sales.Infrastructure.Contexts.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.ToTable("customers");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Email)
            .HasConversion(
                email => email.Value,
                value => new Sales.Domain.ValueObjects.Email(value))
            .IsRequired()
            .HasMaxLength(255);
        entity.Property(e => e.Document)
            .HasConversion(
                doc => doc.Value,
                value => new Document(value))
            .IsRequired()
            .HasMaxLength(50);
        entity.Property(e => e.Password)
            .HasConversion(
                pass => pass.Value,
                value => new Sales.Domain.ValueObjects.Password(value))
            .IsRequired()
            .HasMaxLength(255);
        entity.HasIndex(e => e.Document).IsUnique();
    }
}
