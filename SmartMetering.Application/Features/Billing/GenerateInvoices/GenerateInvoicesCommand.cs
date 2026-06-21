using MediatR;
using SmartMetering.Contracts.Billing;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Billing.GenerateInvoices;

public record GenerateInvoicesCommand(int Month, int Year) : IRequest<Result<GenerateInvoicesResponse>>;
