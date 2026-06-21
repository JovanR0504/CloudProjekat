using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.TariffModels.CreateTariffModel;
using SmartMetering.Application.Features.TariffModels.DeleteTariffModel;
using SmartMetering.Application.Features.TariffModels.GetTariffModels;
using SmartMetering.Application.Features.TariffModels.UpdateTariffModel;
using SmartMetering.Contracts.TariffModels;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Admin,BillingAdmin")]
public class TariffModelsController : BaseApiController
{
    private readonly IMediator _mediator;

    public TariffModelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTariffModelRequest request)
    {
        var command = new CreateTariffModelCommand(
            request.Name,
            request.GreenHighTariffPrice,
            request.GreenLowTariffPrice,
            request.BlueHighTariffPrice,
            request.BlueLowTariffPrice,
            request.RedHighTariffPrice,
            request.RedLowTariffPrice,
            request.GreenZoneUpperLimit,
            request.BlueZoneUpperLimit,
            request.PowerBillingRate,
            request.SupplierFee,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.IsActive);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetTariffModelsQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTariffModelRequest request)
    {
        var command = new UpdateTariffModelCommand(
            id,
            request.Name,
            request.GreenHighTariffPrice,
            request.GreenLowTariffPrice,
            request.BlueHighTariffPrice,
            request.BlueLowTariffPrice,
            request.RedHighTariffPrice,
            request.RedLowTariffPrice,
            request.GreenZoneUpperLimit,
            request.BlueZoneUpperLimit,
            request.PowerBillingRate,
            request.SupplierFee,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.IsActive);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTariffModelCommand(id);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}
