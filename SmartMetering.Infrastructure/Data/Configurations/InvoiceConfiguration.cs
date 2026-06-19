using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.UserId)
            .IsRequired();

        builder.Property(i => i.PropertyId)
            .IsRequired();

        builder.Property(i => i.MeterId)
            .IsRequired();

        builder.Property(i => i.Month)
            .IsRequired();

        builder.Property(i => i.Year)
            .IsRequired();

        builder.Property(i => i.HighTariffKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.LowTariffKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.TotalKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.GreenZoneKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.BlueZoneKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.RedZoneKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(i => i.EnergyCharge)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.PowerCharge)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.SupplierCharge)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.DocumentPath)
            .HasMaxLength(500);

        builder.Property(i => i.StripeSessionId)
            .HasMaxLength(256);

        builder.Property(i => i.StripePaymentId)
            .HasMaxLength(256);

        builder.Property(i => i.CreatedAtUtc)
            .IsRequired();

        builder.Property(i => i.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(i => new { i.UserId, i.MeterId, i.Month, i.Year })
            .IsUnique();

        builder.HasIndex(i => i.UserId);

        builder.Ignore(i => i.DomainEvents);
    }
}
