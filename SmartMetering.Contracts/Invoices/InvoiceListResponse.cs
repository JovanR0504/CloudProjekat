using System.Collections.Generic;

namespace SmartMetering.Contracts.Invoices;

public record InvoiceListResponse(
    List<InvoiceResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
