using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class MeterConfiguration : IEntityTypeConfiguration<Meter>
{
    public void Configure(EntityTypeBuilder<Meter> builder)
    {
        builder.ToTable("Meters");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.PropertyId)
            .IsRequired();

        builder.Property(m => m.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.SerialNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(m => m.SerialNumber)
            .IsUnique();

        builder.Property(m => m.ConnectionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.ApprovedPowerKw)
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.DeviceId)
            .HasMaxLength(100);

        builder.Property(m => m.DeviceTokenHash)
            .HasMaxLength(512);

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(m => m.PropertyId);
        builder.HasIndex(m => m.DeviceId);

        builder.Ignore(m => m.DomainEvents);
    }
}
