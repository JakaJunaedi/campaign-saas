namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class DeliverableRepository : IDeliverableRepository
{
    private readonly DeliverableDbContext _dbContext;

    public DeliverableRepository(DeliverableDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Deliverable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Deliverables
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Deliverable>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Deliverables
            .Where(d => d.CampaignId == campaignId)
            .OrderBy(d => d.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Deliverable>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Deliverables
            .Where(d => d.OrganizationId == organizationId)
            .OrderBy(d => d.DueDate)
            .ToListAsync(cancellationToken);
    }


    public async Task AddAsync(Deliverable deliverable, CancellationToken cancellationToken = default)
    {
        await _dbContext.Deliverables.AddAsync(deliverable, cancellationToken);
    }

    public void Update(Deliverable deliverable)
    {
        _dbContext.Deliverables.Update(deliverable);
    }
}

