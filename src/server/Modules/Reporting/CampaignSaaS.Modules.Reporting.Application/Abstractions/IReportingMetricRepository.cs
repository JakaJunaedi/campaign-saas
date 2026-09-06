namespace CampaignSaaS.Modules.Reporting.Application.Abstractions;

using CampaignSaaS.Modules.Reporting.Domain.Entities;

public interface IReportingMetricRepository
{
    Task<CampaignMetric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CampaignMetric?> GetByDeliverableIdAsync(Guid deliverableId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampaignMetric>> GetByDeliverableIdsAsync(IEnumerable<Guid> deliverableIds, CancellationToken cancellationToken = default);
    Task AddAsync(CampaignMetric metric, CancellationToken cancellationToken = default);
    void Update(CampaignMetric metric);
}
