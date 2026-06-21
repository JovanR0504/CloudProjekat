using MediatR;
using SmartMetering.Contracts.Admin;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Admin.GetInvoiceSummary;

public record GetInvoiceSummaryQuery : IRequest<Result<AdminInvoiceSummaryResponse>>;
