using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AlertType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Severity)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.OccurredAtUtc)
            .IsRequired();

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.Property(a => a.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.MeterId);

        builder.Ignore(a => a.DomainEvents);
    }
}
