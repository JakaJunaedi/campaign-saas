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

    public async Task<CampaignsOverviewStatsDto> GetCampaignsOverviewStatsAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var (allItems, totalCount) = await _campaignRepository.GetPagedAsync(organizationId, null, null, null, 1, 1000, cancellationToken);
        var activeCampaigns = allItems.Where(c => c.Status == Domain.Enums.CampaignStatus.Active).ToList();
        var activeCount = activeCampaigns.Count;
        var activeBudget = activeCampaigns.Sum(c => c.Budget);

        var recentCampaignsList = new List<CampaignSummaryDto>();
        foreach (var c in allItems.Take(5))
        {
            var creatorsCount = await _campaignRepository.GetCreatorsCountAsync(c.Id, cancellationToken);
            recentCampaignsList.Add(new CampaignSummaryDto(
                c.Id,
                c.OrganizationId,
                c.ClientId,
                c.Title,
                c.Budget,
                c.StartDate,
                c.EndDate,
                c.Status.ToString(),
                creatorsCount,
                c.CreatedAt));
        }

        return new CampaignsOverviewStatsDto(
            activeCount,
            totalCount,
            activeBudget,
            recentCampaignsList);
    }
}

