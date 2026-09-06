namespace CampaignSaaS.Modules.Deliverable.Contracts;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;

public interface IDeliverableSummaryQueryService
{
    Task<IReadOnlyList<DeliverableDto>> GetDeliverablesByCampaignIdAsync(Guid organizationId, Guid campaignId, CancellationToken cancellationToken = default);
}
