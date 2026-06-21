using MediatR;
using SmartMetering.Domain.Common;
using System;

namespace SmartMetering.Application.Features.Invoices.DownloadInvoiceDocument;

public record DownloadInvoiceDocumentQuery(Guid InvoiceId)
    : IRequest<Result<InvoiceDocumentResult>>;
