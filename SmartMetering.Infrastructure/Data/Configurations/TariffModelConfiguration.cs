using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data.Configurations;

public class TariffModelConfiguration : IEntityTypeConfiguration<TariffModel>
{
    public void Configure(EntityTypeBuilder<TariffModel> builder)
    {
        builder.ToTable("TariffModels");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.GreenHighTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.GreenLowTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.BlueHighTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.BlueLowTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.RedHighTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.RedLowTariffPrice)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.GreenZoneUpperLimit)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.BlueZoneUpperLimit)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.PowerBillingRate)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(t => t.SupplierFee)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.EffectiveFrom)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired();

        builder.Property(t => t.UpdatedAtUtc)
            .IsRequired();

        builder.Ignore(t => t.DomainEvents);
    }
}
