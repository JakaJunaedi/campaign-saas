namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Infrastructure.Persistence;

public class ReportingUnitOfWork : IReportingUnitOfWork
{
    private readonly ReportingDbContext _dbContext;

    public ReportingUnitOfWork(ReportingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
