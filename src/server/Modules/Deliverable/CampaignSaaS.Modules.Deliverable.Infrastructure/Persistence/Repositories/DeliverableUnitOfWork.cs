namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;

public class DeliverableUnitOfWork : IDeliverableUnitOfWork
{
    private readonly DeliverableDbContext _dbContext;

    public DeliverableUnitOfWork(DeliverableDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

