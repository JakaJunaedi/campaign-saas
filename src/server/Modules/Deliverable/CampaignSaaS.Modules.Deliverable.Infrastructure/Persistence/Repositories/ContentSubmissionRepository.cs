namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ContentSubmissionRepository : IContentSubmissionRepository
{
    private readonly DeliverableDbContext _dbContext;

    public ContentSubmissionRepository(DeliverableDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ContentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ContentSubmissions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ContentSubmission>> GetByDeliverableIdAsync(Guid deliverableId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ContentSubmissions
            .Where(s => s.DeliverableId == deliverableId)
            .OrderBy(s => s.VersionNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetLatestVersionNumberAsync(Guid deliverableId, CancellationToken cancellationToken = default)
    {
        var maxVersion = await _dbContext.ContentSubmissions
            .Where(s => s.DeliverableId == deliverableId)
            .MaxAsync(s => (int?)s.VersionNumber, cancellationToken);

        return maxVersion ?? 0;
    }

    public async Task AddAsync(ContentSubmission submission, CancellationToken cancellationToken = default)
    {
        await _dbContext.ContentSubmissions.AddAsync(submission, cancellationToken);
    }
}

