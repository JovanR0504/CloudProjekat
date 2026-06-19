using Microsoft.EntityFrameworkCore;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Property> Properties { get; }
    DbSet<Meter> Meters { get; }
    DbSet<PasswordToken> PasswordTokens { get; }
    DbSet<TariffModel> TariffModels { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Alert> Alerts { get; }
    DbSet<ConsumptionLimit> ConsumptionLimits { get; }
    DbSet<ManualReading> ManualReadings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
