namespace CampaignSaaS.Modules.Deliverable.Application.Abstractions;

using CampaignSaaS.Modules.Deliverable.Domain.Entities;

public interface IContentSubmissionRepository
{
    Task<ContentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentSubmission>> GetByDeliverableIdAsync(Guid deliverableId, CancellationToken cancellationToken = default);
    Task<int> GetLatestVersionNumberAsync(Guid deliverableId, CancellationToken cancellationToken = default);
    Task AddAsync(ContentSubmission submission, CancellationToken cancellationToken = default);
}

