namespace CampaignSaaS.Modules.Reporting.Application.Queries.GetDeliverableMetrics;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class GetDeliverableMetricsQueryHandler : IRequestHandler<GetDeliverableMetricsQuery, ErrorOr<CampaignMetricDto>>
{
    private readonly IReportingMetricRepository _metricRepository;
    private readonly ICurrentTenantContext _tenantContext;

    public GetDeliverableMetricsQueryHandler(
        IReportingMetricRepository metricRepository,
        ICurrentTenantContext tenantContext)
    {
        _metricRepository = metricRepository;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignMetricDto>> Handle(GetDeliverableMetricsQuery request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var metric = await _metricRepository.GetByDeliverableIdAsync(request.DeliverableId, cancellationToken);
        if (metric == null)
        {
            return Error.NotFound("CampaignMetric.NotFound", "Metrics not found for the specified deliverable.");
        }

        return new CampaignMetricDto(
            metric.Id,
            metric.OrganizationId,
            metric.DeliverableId,
            metric.Reach,
            metric.Impressions,
            metric.Views,
            metric.Likes,
            metric.Comments,
            metric.Shares,
            metric.Clicks,
            metric.TotalEngagement,
            metric.CalculateEngagementRate(),
            metric.RecordedAt);
    }
}
