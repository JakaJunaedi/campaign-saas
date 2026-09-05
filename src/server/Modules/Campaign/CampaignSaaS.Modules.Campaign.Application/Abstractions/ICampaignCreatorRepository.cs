namespace CampaignSaaS.Modules.Campaign.Application.Abstractions;

using CampaignSaaS.Modules.Campaign.Domain.Entities;

public interface ICampaignCreatorRepository
{
    Task<CampaignCreator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CampaignCreator?> GetByCampaignAndCreatorAsync(Guid campaignId, Guid creatorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampaignCreator>> GetRosterByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<bool> ExistsInCampaignAsync(Guid campaignId, Guid creatorId, CancellationToken cancellationToken = default);
    Task AddAsync(CampaignCreator rosterItem, CancellationToken cancellationToken = default);
    void Update(CampaignCreator rosterItem);
    void Remove(CampaignCreator rosterItem);
}
