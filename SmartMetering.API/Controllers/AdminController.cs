using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Admin.GetInvoiceSummary;
using SmartMetering.Application.Features.Admin.GetMeterOverview;
using SmartMetering.Application.Features.Billing.GenerateInvoices;
using SmartMetering.Contracts.Billing;

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

    [HttpPost("generate-invoices")]
    public async Task<IActionResult> GenerateInvoices([FromBody] GenerateInvoicesRequest request)
    {
        var command = new GenerateInvoicesCommand(request.Month, request.Year);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}
