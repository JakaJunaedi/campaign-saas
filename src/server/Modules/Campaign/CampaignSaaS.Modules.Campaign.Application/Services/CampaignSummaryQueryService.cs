namespace CampaignSaaS.Modules.Campaign.Application.Services;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Contracts;
using CampaignSaaS.Modules.Campaign.Contracts.DTOs;

public class CampaignSummaryQueryService : ICampaignSummaryQueryService
{
    private readonly ICampaignRepository _campaignRepository;

    public CampaignSummaryQueryService(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<CampaignDto?> GetCampaignByIdAsync(Guid organizationId, Guid campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId, cancellationToken);
        if (campaign == null || campaign.OrganizationId != organizationId)
        {
            return null;
        }

        return new CampaignDto(
            campaign.Id,
            campaign.OrganizationId,
            campaign.ClientId,
            campaign.Title,
            campaign.Description,
            campaign.Budget,
            campaign.StartDate,
            campaign.EndDate,
            campaign.Status.ToString(),
            campaign.CreatedAt,
            campaign.UpdatedAt);
    }
}
