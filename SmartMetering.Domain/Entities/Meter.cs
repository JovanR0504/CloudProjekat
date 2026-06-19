using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;
using SmartMetering.Domain.Helpers;

namespace SmartMetering.Domain.Entities;

public class Meter : AggregateRoot
{
    public Guid PropertyId { get; private set; }
    public string Label { get; private set; } = null!;
    public string SerialNumber { get; private set; } = null!;
    public ConnectionType ConnectionType { get; private set; }
    public double ApprovedPowerKw { get; private set; }
    public string? Notes { get; private set; }
    public MeterStatus Status { get; private set; }
    public string? DeviceId { get; private set; }
    public string? DeviceTokenHash { get; private set; }
    public DateTime? LastActiveAtUtc { get; private set; }

    private Meter() { }

    public static Result<Meter> Create(
        Guid propertyId,
        string label,
        string serialNumber,
        ConnectionType connectionType,
        string? notes)
    {
        if (propertyId == Guid.Empty)
            return Result<Meter>.Failure(Error.Validation("Meter.EmptyPropertyId", "Property ID is required."));

        if (string.IsNullOrWhiteSpace(label))
            return Result<Meter>.Failure(Error.Validation("Meter.EmptyLabel", "Label is required."));

        if (string.IsNullOrWhiteSpace(serialNumber))
            return Result<Meter>.Failure(Error.Validation("Meter.EmptySerialNumber", "Serial number is required."));

        if (!SerialNumberValidator.IsValid(serialNumber))
            return Result<Meter>.Failure(Error.Validation("Meter.InvalidSerialNumber", "Serial number must match format SM-YYYY-XXXXX."));

        double approvedPower = connectionType == ConnectionType.SinglePhase ? 6.9 : 11.04;

        var meter = new Meter
        {
            PropertyId = propertyId,
            Label = label.Trim(),
            SerialNumber = serialNumber.Trim().ToUpperInvariant(),
            ConnectionType = connectionType,
            ApprovedPowerKw = approvedPower,
            Notes = notes?.Trim(),
            Status = MeterStatus.Registered
        };

        return Result<Meter>.Success(meter);
    }

    public Result Activate(string deviceId, string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return Result.Failure(Error.Validation("Meter.EmptyDeviceId", "Device ID is required."));

        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result.Failure(Error.Validation("Meter.EmptyTokenHash", "Device token hash is required."));

        DeviceId = deviceId;
        DeviceTokenHash = tokenHash;
        Status = MeterStatus.Active;
        LastActiveAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public void MarkActive(DateTime timestamp)
    {
        LastActiveAtUtc = timestamp;
        if (Status != MeterStatus.Active)
            Status = MeterStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkInactive()
    {
        Status = MeterStatus.Inactive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateInfo(string label, string? notes)
    {
        Label = label.Trim();
        Notes = notes?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
