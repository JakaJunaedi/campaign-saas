namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator;
using CampaignSaaS.Modules.Creator.Application.Commands.DeleteCreator;
using CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreator;
using CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreatorStatus;
using CampaignSaaS.Modules.Creator.Application.Queries.GetCreatorById;
using CampaignSaaS.Modules.Creator.Application.Queries.GetCreators;
using CampaignSaaS.Modules.Creator.Contracts.DTOs;
using CampaignSaaS.Modules.Creator.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
[Route("api/v1/creators")]
public class CreatorController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CreatorSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCreators(
        [FromQuery] string? search = null,
        [FromQuery] string? niche = null,
        [FromQuery] string? platform = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCreatorsQuery(search, niche, platform, status, page, pageSize);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CreatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCreatorById(Guid id)
    {
        var query = new GetCreatorByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCreator([FromBody] CreateCreatorRequest request)
    {
        var command = new CreateCreatorCommand(
            request.FullName,
            request.Niche,
            request.Email,
            request.PhoneNumber,
            request.SocialAccounts);

        var result = await Mediator.Send(command);
        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CreatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCreator(Guid id, [FromBody] UpdateCreatorRequest request)
    {
        var command = new UpdateCreatorCommand(
            id,
            request.FullName,
            request.Niche,
            request.Email,
            request.PhoneNumber,
            request.SocialAccounts);

        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<CreatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCreatorStatus(Guid id, [FromBody] UpdateCreatorStatusRequest request)
    {
        var command = new UpdateCreatorStatusCommand(id, request.Status);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCreator(Guid id)
    {
        var command = new DeleteCreatorCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}
