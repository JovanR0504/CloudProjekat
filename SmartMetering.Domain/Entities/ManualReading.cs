using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class ManualReading : AggregateRoot
{
    public Guid MeterId { get; private set; }
    public Guid UserId { get; private set; }
    public double Reading { get; private set; }
    public string PhotoUrl { get; private set; } = null!;
    public ManualReadingStatus Status { get; private set; }
    public DateTime? VerifiedAtUtc { get; private set; }
    public string? VerificationNotes { get; private set; }

    private ManualReading() { }

    public static Result<ManualReading> Create(
        Guid meterId,
        Guid userId,
        double reading,
        string photoUrl)
    {
        if (meterId == Guid.Empty)
            return Result<ManualReading>.Failure(Error.Validation("ManualReading.EmptyMeterId", "Meter ID is required."));

        if (userId == Guid.Empty)
            return Result<ManualReading>.Failure(Error.Validation("ManualReading.EmptyUserId", "User ID is required."));

        if (reading < 0)
            return Result<ManualReading>.Failure(Error.Validation("ManualReading.NegativeReading", "Reading cannot be negative."));

        if (string.IsNullOrWhiteSpace(photoUrl))
            return Result<ManualReading>.Failure(Error.Validation("ManualReading.EmptyPhotoUrl", "Photo URL is required."));

        var manualReading = new ManualReading
        {
            MeterId = meterId,
            UserId = userId,
            Reading = reading,
            PhotoUrl = photoUrl.Trim(),
            Status = ManualReadingStatus.Pending
        };

        return Result<ManualReading>.Success(manualReading);
    }

    public void Approve(string? notes)
    {
        Status = ManualReadingStatus.Approved;
        VerifiedAtUtc = DateTime.UtcNow;
        VerificationNotes = notes?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Reject(string? notes)
    {
        Status = ManualReadingStatus.Rejected;
        VerifiedAtUtc = DateTime.UtcNow;
        VerificationNotes = notes?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
