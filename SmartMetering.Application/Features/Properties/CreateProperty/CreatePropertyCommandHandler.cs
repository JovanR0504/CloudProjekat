using MediatR;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Application.Features.Properties.CreateProperty;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<PropertyResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreatePropertyCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PropertyResponse>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyResult = Property.Create(
            _currentUser.UserId,
            request.Name,
            request.City,
            request.Address,
            request.Description);

        if (propertyResult.IsFailure)
            return Result<PropertyResponse>.Failure(propertyResult.Error!);

        var property = propertyResult.Value;
        _context.Properties.Add(property);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new PropertyResponse(
            property.Id,
            property.Name,
            property.City,
            property.Address,
            property.Description,
            property.UserId,
            property.CreatedAtUtc);

        return Result<PropertyResponse>.Success(response);
    }
}
