using MediatR;
using SmartMetering.Contracts.Invoices;
using SmartMetering.Domain.Common;
using System;

namespace SmartMetering.Application.Features.Invoices.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<Result<InvoiceDetailResponse>>;
