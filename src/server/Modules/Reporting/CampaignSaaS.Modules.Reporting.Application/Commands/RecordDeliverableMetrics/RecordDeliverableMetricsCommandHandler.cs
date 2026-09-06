namespace CampaignSaaS.Modules.Reporting.Application.Commands.RecordDeliverableMetrics;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class RecordDeliverableMetricsCommandHandler : IRequestHandler<RecordDeliverableMetricsCommand, ErrorOr<CampaignMetricDto>>
{
    private readonly IReportingMetricRepository _metricRepository;
    private readonly IReportingUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public RecordDeliverableMetricsCommandHandler(
        IReportingMetricRepository metricRepository,
        IReportingUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _metricRepository = metricRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignMetricDto>> Handle(RecordDeliverableMetricsCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var existingMetric = await _metricRepository.GetByDeliverableIdAsync(request.DeliverableId, cancellationToken);
        if (existingMetric != null)
        {
            existingMetric.UpdateMetrics(
                request.Reach,
                request.Impressions,
                request.Views,
                request.Likes,
                request.Comments,
                request.Shares,
                request.Clicks);

            _metricRepository.Update(existingMetric);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CampaignMetricDto(
                existingMetric.Id,
                existingMetric.OrganizationId,
                existingMetric.DeliverableId,
                existingMetric.Reach,
                existingMetric.Impressions,
                existingMetric.Views,
                existingMetric.Likes,
                existingMetric.Comments,
                existingMetric.Shares,
                existingMetric.Clicks,
                existingMetric.TotalEngagement,
                existingMetric.CalculateEngagementRate(),
                existingMetric.RecordedAt);
        }

        var newMetric = CampaignMetric.Record(
            orgId,
            request.DeliverableId,
            request.Reach,
            request.Impressions,
            request.Views,
            request.Likes,
            request.Comments,
            request.Shares,
            request.Clicks);

        await _metricRepository.AddAsync(newMetric, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CampaignMetricDto(
            newMetric.Id,
            newMetric.OrganizationId,
            newMetric.DeliverableId,
            newMetric.Reach,
            newMetric.Impressions,
            newMetric.Views,
            newMetric.Likes,
            newMetric.Comments,
            newMetric.Shares,
            newMetric.Clicks,
            newMetric.TotalEngagement,
            newMetric.CalculateEngagementRate(),
            newMetric.RecordedAt);
    }
}
