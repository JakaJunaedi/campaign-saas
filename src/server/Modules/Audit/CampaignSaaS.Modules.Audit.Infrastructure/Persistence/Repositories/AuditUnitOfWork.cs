namespace CampaignSaaS.Modules.Audit.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Infrastructure.Persistence;

public class AuditUnitOfWork : IAuditUnitOfWork
{
    private readonly AuditDbContext _dbContext;

    public AuditUnitOfWork(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
