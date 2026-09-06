namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Campaign.Application.Commands.AddCreatorToRoster;
using CampaignSaaS.Modules.Campaign.Application.Commands.CreateCampaign;
using CampaignSaaS.Modules.Campaign.Application.Commands.DeleteCampaign;
using CampaignSaaS.Modules.Campaign.Application.Commands.RemoveCreatorFromRoster;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaign;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaignStatus;
using CampaignSaaS.Modules.Campaign.Application.Commands.UpdateRosterStatus;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignById;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaignRoster;
using CampaignSaaS.Modules.Campaign.Application.Queries.GetCampaigns;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;
using CampaignSaaS.Modules.Campaign.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
[Route("api/v1/campaigns")]
public class CampaignController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CampaignSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaigns(
        [FromQuery] string? search = null,
        [FromQuery] Guid? clientId = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCampaignsQuery(search, clientId, status, page, pageSize);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignById(Guid id)
    {
        var query = new GetCampaignByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        var command = new CreateCampaignCommand(
            request.ClientId,
            request.Title,
            request.Description,
            request.Budget,
            request.StartDate,
            request.EndDate);

        var result = await Mediator.Send(command);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        var command = new UpdateCampaignCommand(
            id,
            request.Title,
            request.Description,
            request.Budget,
            request.StartDate,
            request.EndDate);

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCampaignStatus(Guid id, [FromBody] UpdateCampaignStatusRequest request)
    {
        var command = new UpdateCampaignStatusCommand(id, request.Status);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaign(Guid id)
    {
        var command = new DeleteCampaignCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    // Roster Endpoints
    [HttpGet("{id:guid}/roster")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CampaignCreatorDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignRoster(Guid id)
    {
        var query = new GetCampaignRosterQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/roster")]
    [ProducesResponseType(typeof(ApiResponse<CampaignCreatorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCreatorToRoster(Guid id, [FromBody] AddCreatorToRosterRequest request)
    {
        var command = new AddCreatorToRosterCommand(id, request.CreatorId, request.AgreedRate);
        var result = await Mediator.Send(command);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpPatch("{id:guid}/roster/{creatorId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<CampaignCreatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRosterStatus(Guid id, Guid creatorId, [FromBody] UpdateRosterStatusRequest request)
    {
        var command = new UpdateRosterStatusCommand(id, creatorId, request.Status, request.AgreedRate);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/roster/{creatorId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCreatorFromRoster(Guid id, Guid creatorId)
    {
        var command = new RemoveCreatorFromRosterCommand(id, creatorId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
