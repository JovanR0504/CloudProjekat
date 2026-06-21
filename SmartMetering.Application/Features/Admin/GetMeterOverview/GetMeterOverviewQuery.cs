using MediatR;
using SmartMetering.Contracts.Admin;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Admin.GetMeterOverview;

public record GetMeterOverviewQuery : IRequest<Result<List<MeterOverviewResponse>>>;
