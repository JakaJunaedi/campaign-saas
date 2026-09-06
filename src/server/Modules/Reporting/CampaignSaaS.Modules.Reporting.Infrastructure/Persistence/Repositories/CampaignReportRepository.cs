namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Repositories;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.Modules.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CampaignReportRepository : ICampaignReportRepository
{
    private readonly ReportingDbContext _dbContext;

    public CampaignReportRepository(ReportingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CampaignReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CampaignReports
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignReport>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CampaignReports
            .Where(r => r.CampaignId == campaignId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalReportsCountAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CampaignReports
            .Where(r => r.OrganizationId == organizationId)
            .CountAsync(cancellationToken);
    }


    public async Task AddAsync(CampaignReport report, CancellationToken cancellationToken = default)
    {
        await _dbContext.CampaignReports.AddAsync(report, cancellationToken);
    }

    public void Update(CampaignReport report)
    {
        _dbContext.CampaignReports.Update(report);
    }
}
