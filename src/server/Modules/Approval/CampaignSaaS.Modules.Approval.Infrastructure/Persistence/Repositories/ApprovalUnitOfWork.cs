namespace CampaignSaaS.Modules.Approval.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Approval.Application.Abstractions;
using CampaignSaaS.Modules.Approval.Infrastructure.Persistence;

public class ApprovalUnitOfWork : IApprovalUnitOfWork
{
    private readonly ApprovalDbContext _dbContext;

    public ApprovalUnitOfWork(ApprovalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
