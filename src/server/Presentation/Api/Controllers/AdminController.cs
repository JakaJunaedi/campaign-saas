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

    [HttpPost("organizations")]
    [ProducesResponseType(typeof(ApiResponse<AdminOrganizationItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateOrganization(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CampaignSaaS.Modules.Identity.Application.Commands.CreateOrganization.CreateOrganizationCommand(
            request.OrganizationName,
            request.Slug,
            request.AdminFullName,
            request.AdminEmail,
            request.Password);
        
        var result = await Mediator.Send(command, cancellationToken);
        // Using Created/201 response pattern would ideally require location URL, but for simple MVP using Created with payload
        if (result.IsError)
        {
            return HandleResult(result);
        }
        
        return Created(string.Empty, ApiResponse<AdminOrganizationItemDto>.Ok(result.Value));
    }

    [HttpPut("organizations/{id:guid}/quota")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrganizationQuota(
        Guid id,
        [FromBody] UpdateOrganizationQuotaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationQuota.UpdateOrganizationQuotaCommand(
            id,
            request.StorageQuotaBytes,
            request.MaxActiveCampaigns);
            
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("impersonate/{orgId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Impersonate(
        Guid orgId,
        CancellationToken cancellationToken)
    {
        var command = new CampaignSaaS.Modules.Identity.Application.Commands.ImpersonateTenant.ImpersonateTenantCommand(orgId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(ApiResponse<CampaignSaaS.Modules.Audit.Contracts.DTOs.PagedResult<CampaignSaaS.Modules.Audit.Contracts.DTOs.AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? module = null,
        [FromQuery] string? action = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new CampaignSaaS.Modules.Audit.Application.Queries.GetAuditLogs.GetAuditLogsQuery(module, action, entityId, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
