using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Admin;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Admin.GetInvoiceSummary;

public class GetInvoiceSummaryQueryHandler : IRequestHandler<GetInvoiceSummaryQuery, Result<AdminInvoiceSummaryResponse>>
{
    private readonly IAppDbContext _context;

    public GetInvoiceSummaryQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AdminInvoiceSummaryResponse>> Handle(GetInvoiceSummaryQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalInvoices = invoices.Count;
        var paidCount = invoices.Count(i => i.Status == InvoiceStatus.Paid);
        var unpaidCount = invoices.Count(i => i.Status == InvoiceStatus.Unpaid);
        var totalBilled = invoices.Sum(i => i.TotalAmount);
        var totalCollected = invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.TotalAmount);

        var response = new AdminInvoiceSummaryResponse(
            totalInvoices,
            paidCount,
            unpaidCount,
            totalBilled,
            totalCollected);

        return Result<AdminInvoiceSummaryResponse>.Success(response);
    }
}
