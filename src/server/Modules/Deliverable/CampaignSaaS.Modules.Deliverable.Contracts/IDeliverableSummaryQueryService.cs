namespace CampaignSaaS.Modules.Deliverable.Contracts;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;

public interface IDeliverableSummaryQueryService
{
    Task<IReadOnlyList<DeliverableDto>> GetDeliverablesByCampaignIdAsync(Guid organizationId, Guid campaignId, CancellationToken cancellationToken = default);
    Task<DeliverablesOverviewStatsDto> GetDeliverablesOverviewStatsAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

