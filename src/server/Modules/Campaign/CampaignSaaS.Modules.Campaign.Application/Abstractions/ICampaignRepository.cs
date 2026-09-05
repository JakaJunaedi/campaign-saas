namespace CampaignSaaS.Modules.Campaign.Application.Abstractions;

using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        Guid? clientId,
        CampaignStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<int> GetCreatorsCountAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    void Update(Campaign campaign);
}
