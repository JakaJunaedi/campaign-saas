namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.Modules.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ReportingMetricRepository : IReportingMetricRepository
{
    private readonly ReportingDbContext _dbContext;

    public ReportingMetricRepository(ReportingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CampaignMetric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CampaignMetrics
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<CampaignMetric?> GetByDeliverableIdAsync(Guid deliverableId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CampaignMetrics
            .FirstOrDefaultAsync(m => m.DeliverableId == deliverableId, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignMetric>> GetByDeliverableIdsAsync(IEnumerable<Guid> deliverableIds, CancellationToken cancellationToken = default)
    {
        var idList = deliverableIds.ToList();
        return await _dbContext.CampaignMetrics
            .Where(m => idList.Contains(m.DeliverableId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CampaignMetric metric, CancellationToken cancellationToken = default)
    {
        await _dbContext.CampaignMetrics.AddAsync(metric, cancellationToken);
    }

    public void Update(CampaignMetric metric)
    {
        _dbContext.CampaignMetrics.Update(metric);
    }
}
