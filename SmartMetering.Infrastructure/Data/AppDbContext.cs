using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Meter> Meters => Set<Meter>();
    public DbSet<PasswordToken> PasswordTokens => Set<PasswordToken>();
    public DbSet<TariffModel> TariffModels => Set<TariffModel>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<ConsumptionLimit> ConsumptionLimits => Set<ConsumptionLimit>();
    public DbSet<ManualReading> ManualReadings => Set<ManualReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.Entity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.GetType()
                    .GetProperty(nameof(Domain.Common.Entity.UpdatedAtUtc))!
                    .SetValue(entry.Entity, DateTime.UtcNow);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
