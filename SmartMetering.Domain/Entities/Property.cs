using SmartMetering.Domain.Common;

namespace SmartMetering.Domain.Entities;

public class Property : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? Description { get; private set; }

    private Property() { }

    public static Result<Property> Create(
        Guid userId,
        string name,
        string city,
        string address,
        string? description)
    {
        if (userId == Guid.Empty)
            return Result<Property>.Failure(Error.Validation("Property.EmptyUserId", "User ID is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result<Property>.Failure(Error.Validation("Property.EmptyName", "Property name is required."));

        if (string.IsNullOrWhiteSpace(city))
            return Result<Property>.Failure(Error.Validation("Property.EmptyCity", "City is required."));

        if (string.IsNullOrWhiteSpace(address))
            return Result<Property>.Failure(Error.Validation("Property.EmptyAddress", "Address is required."));

        var property = new Property
        {
            UserId = userId,
            Name = name.Trim(),
            City = city.Trim(),
            Address = address.Trim(),
            Description = description?.Trim()
        };

        return Result<Property>.Success(property);
    }

    public void Update(string name, string city, string address, string? description)
    {
        Name = name.Trim();
        City = city.Trim();
        Address = address.Trim();
        Description = description?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
