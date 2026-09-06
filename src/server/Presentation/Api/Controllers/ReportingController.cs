namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;
using CampaignSaaS.Modules.Reporting.Application.Commands.RecordDeliverableMetrics;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetCampaignReports;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetDeliverableMetrics;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetReportById;
using CampaignSaaS.Modules.Reporting.Application.Queries.GetReportDownloadUrl;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer")]
[ApiController]
public class ReportingController : ApiControllerBase
{
    // --- Deliverable Metrics Endpoints (Phase 9) ---

    [HttpPut("api/v1/deliverables/{deliverableId:guid}/metrics")]
    [ProducesResponseType(typeof(ApiResponse<CampaignMetricDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RecordDeliverableMetrics(
        Guid deliverableId,
        [FromBody] RecordDeliverableMetricsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RecordDeliverableMetricsCommand(
            deliverableId,
            request.Reach,
            request.Impressions,
            request.Views,
            request.Likes,
            request.Comments,
            request.Shares,
            request.Clicks);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("api/v1/deliverables/{deliverableId:guid}/metrics")]
    [ProducesResponseType(typeof(ApiResponse<CampaignMetricDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliverableMetrics(
        Guid deliverableId,
        CancellationToken cancellationToken)
    {
        var query = new GetDeliverableMetricsQuery(deliverableId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    // --- Campaign Report Generation Endpoints (Phase 10) ---

    [HttpPost("api/v1/campaigns/{campaignId:guid}/reports/generate")]
    [ProducesResponseType(typeof(ApiResponse<CampaignReportDto>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GenerateCampaignReport(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var command = new GenerateCampaignReportCommand(campaignId);
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result, StatusCodes.Status202Accepted);
    }

    [HttpGet("api/v1/campaigns/{campaignId:guid}/reports")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CampaignReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignReports(
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var query = new GetCampaignReportsQuery(campaignId);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("api/v1/reports/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReportByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("api/v1/reports/{id:guid}/download")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportDownloadUrl(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetReportDownloadUrlQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
