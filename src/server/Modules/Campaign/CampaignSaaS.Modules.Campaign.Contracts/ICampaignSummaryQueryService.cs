namespace CampaignSaaS.Modules.Campaign.Contracts;

using CampaignSaaS.Modules.Campaign.Contracts.DTOs;

public interface ICampaignSummaryQueryService
{
    Task<CampaignDto?> GetCampaignByIdAsync(Guid organizationId, Guid campaignId, CancellationToken cancellationToken = default);
}
