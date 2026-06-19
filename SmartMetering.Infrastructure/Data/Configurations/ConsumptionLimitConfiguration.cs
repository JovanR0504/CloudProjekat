using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class ConsumptionLimitConfiguration : IEntityTypeConfiguration<ConsumptionLimit>
{
    public void Configure(EntityTypeBuilder<ConsumptionLimit> builder)
    {
        builder.ToTable("ConsumptionLimits");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Threshold)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Unit)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.NotifiedForMonth)
            .HasMaxLength(10);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(c => c.UserId);

        builder.Ignore(c => c.DomainEvents);
    }
}
