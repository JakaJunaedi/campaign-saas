namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Audit.Application.Queries.GetAuditLogs;
using CampaignSaaS.Modules.Audit.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager")]
[ApiController]
public class AuditController : ApiControllerBase
{
    [HttpGet("api/v1/audit-logs")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? module,
        [FromQuery] string? action,
        [FromQuery] Guid? entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuditLogsQuery(module, action, entityId, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
