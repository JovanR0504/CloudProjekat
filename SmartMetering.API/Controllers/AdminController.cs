using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Admin.GetInvoiceSummary;
using SmartMetering.Application.Features.Admin.GetMeterOverview;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Admin,BillingAdmin")]
public class AdminController : BaseApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("meters")]
    public async Task<IActionResult> GetMeterOverview()
    {
        var query = new GetMeterOverviewQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("invoices-summary")]
    public async Task<IActionResult> GetInvoiceSummary()
    {
        var query = new GetInvoiceSummaryQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}
