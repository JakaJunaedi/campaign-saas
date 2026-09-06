namespace CampaignSaaS.Modules.Deliverable.Application.Abstractions;

using CampaignSaaS.Modules.Deliverable.Domain.Entities;

public interface IDeliverableRepository
{
    Task<Deliverable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deliverable>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Deliverable>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(Deliverable deliverable, CancellationToken cancellationToken = default);
    void Update(Deliverable deliverable);
}


