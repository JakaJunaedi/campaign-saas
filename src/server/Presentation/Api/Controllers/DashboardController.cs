namespace CampaignSaaS.Api.Controllers;

using CampaignSaaS.Api.Common;
using CampaignSaaS.Modules.Campaign.Contracts;
using CampaignSaaS.Modules.Client.Contracts;
using CampaignSaaS.Modules.Creator.Contracts;
using CampaignSaaS.Modules.Deliverable.Contracts;
using CampaignSaaS.Modules.Reporting.Contracts;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public sealed record DashboardOverviewDto(
    int ActiveCampaignsCount,
    int TotalCampaignsCount,
    int TotalClientsCount,
    int TotalCreatorsCount,
    int PendingReviewsCount,
    int CompletedDeliverablesCount,
    decimal TotalBudgetManaged,
    int TotalReportsCount,
    IReadOnlyList<DashboardRecentCampaignItemDto> RecentCampaigns,
    IReadOnlyList<DashboardActionItemDto> ActionItems
);

public sealed record DashboardRecentCampaignItemDto(
    Guid Id,
    string Title,
    string ClientName,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Budget,
    string Status,
    int CreatorsCount
);

public sealed record DashboardActionItemDto(
    Guid DeliverableId,
    Guid CampaignId,
    string Title,
    string Platform,
    string ContentType,
    string Status,
    DateOnly DueDate
);

[Authorize(Roles = "SuperAdmin,AgencyOwner,CampaignManager,ContentReviewer,ClientViewer")]
[Route("api/v1/dashboard")]
public class DashboardController : ApiControllerBase
{
    private readonly ICurrentTenantContext _tenantContext;
    private readonly ICampaignSummaryQueryService _campaignSummary;
    private readonly IClientSummaryQueryService _clientSummary;
    private readonly ICreatorSummaryQueryService _creatorSummary;
    private readonly IDeliverableSummaryQueryService _deliverableSummary;
    private readonly IReportingSummaryQueryService _reportingSummary;

    public DashboardController(
        ICurrentTenantContext tenantContext,
        ICampaignSummaryQueryService campaignSummary,
        IClientSummaryQueryService clientSummary,
        ICreatorSummaryQueryService creatorSummary,
        IDeliverableSummaryQueryService deliverableSummary,
        IReportingSummaryQueryService reportingSummary)
    {
        _tenantContext = tenantContext;
        _campaignSummary = campaignSummary;
        _clientSummary = clientSummary;
        _creatorSummary = creatorSummary;
        _deliverableSummary = deliverableSummary;
        _reportingSummary = reportingSummary;
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var orgId = _tenantContext.OrganizationId;
        if (!orgId.HasValue || orgId.Value == Guid.Empty)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Missing Tenant Context",
                Detail = "No active organization found in security context.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var campaignsTask = _campaignSummary.GetCampaignsOverviewStatsAsync(orgId.Value, cancellationToken);
        var clientsTask = _clientSummary.GetClientsCountAsync(orgId.Value, cancellationToken);
        var creatorsTask = _creatorSummary.GetCreatorsCountAsync(orgId.Value, cancellationToken);
        var deliverablesTask = _deliverableSummary.GetDeliverablesOverviewStatsAsync(orgId.Value, cancellationToken);
        var reportsTask = _reportingSummary.GetReportsCountAsync(orgId.Value, cancellationToken);

        await Task.WhenAll(campaignsTask, clientsTask, creatorsTask, deliverablesTask, reportsTask);

        var campaigns = await campaignsTask;
        var clientsCount = await clientsTask;
        var creatorsCount = await creatorsTask;
        var deliverables = await deliverablesTask;
        var reportsCount = await reportsTask;

        var recentCampaignItems = new List<DashboardRecentCampaignItemDto>();
        foreach (var c in campaigns.RecentCampaigns)
        {
            var clientName = await _clientSummary.GetClientNameAsync(orgId.Value, c.ClientId, cancellationToken) ?? "Unknown Client";
            recentCampaignItems.Add(new DashboardRecentCampaignItemDto(
                c.Id,
                c.Title,
                clientName,
                c.StartDate,
                c.EndDate,
                c.Budget,
                c.Status,
                c.CreatorsCount));
        }

        var actionItems = deliverables.ActionItems.Select(a => new DashboardActionItemDto(
            a.DeliverableId,
            a.CampaignId,
            a.DeliverableTitle,
            a.Platform,
            a.ContentType,
            a.Status,
            a.DueDate)).ToList();

        var dto = new DashboardOverviewDto(
            campaigns.ActiveCount,
            campaigns.TotalCount,
            clientsCount,
            creatorsCount,
            deliverables.PendingReviewsCount,
            deliverables.CompletedDeliverablesCount,
            campaigns.ActiveBudget,
            reportsCount,
            recentCampaignItems,
            actionItems);

        return Ok(ApiResponse<DashboardOverviewDto>.Ok(dto));
    }
}

