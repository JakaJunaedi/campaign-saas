namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationStatus;
using CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOrganizations;
using CampaignSaaS.Modules.Identity.Application.Queries.GetAdminOverview;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin")]
[Route("api/v1/admin")]
public class AdminController : ApiControllerBase
{
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<AdminOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var query = new GetAdminOverviewQuery();
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("organizations")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminOrganizationItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganizations(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminOrganizationsQuery(search, status, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("organizations/{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<AdminOrganizationItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrganizationStatus(
        Guid id,
        [FromBody] UpdateOrganizationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrganizationStatusCommand(id, request.Status);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
