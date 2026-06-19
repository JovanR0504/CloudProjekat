using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Infrastructure.Data;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        AppDbContext context,
        IPasswordService passwordService,
        IConfiguration configuration,
        ILogger<DbSeeder> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        var adminEmail = _configuration["Seed:AdminEmail"] ?? "admin@smartmetering.local";
        var adminPassword = _configuration["Seed:AdminPassword"] ?? "Admin!2345";

        var passwordHash = _passwordService.Hash(adminPassword);

        var adminResult = User.Create(
            adminEmail,
            passwordHash,
            "System",
            "Administrator",
            "+381600000000",
            UserRole.Admin);

        if (adminResult.IsFailure)
        {
            _logger.LogError("Failed to create admin user: {Error}", adminResult.Error!.Message);
            return;
        }

        var admin = adminResult.Value;
        admin.Activate();

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }
}
