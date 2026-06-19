using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class ManualReadingConfiguration : IEntityTypeConfiguration<ManualReading>
{
    public void Configure(EntityTypeBuilder<ManualReading> builder)
    {
        builder.ToTable("ManualReadings");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MeterId)
            .IsRequired();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.Reading)
            .IsRequired();

        builder.Property(m => m.PhotoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.VerificationNotes)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedAtUtc)
            .IsRequired();

        builder.Property(m => m.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(m => m.MeterId);
        builder.HasIndex(m => m.UserId);

        builder.Ignore(m => m.DomainEvents);
    }
}
