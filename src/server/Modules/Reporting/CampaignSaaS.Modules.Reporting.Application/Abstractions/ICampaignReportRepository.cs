namespace CampaignSaaS.Modules.Reporting.Application.Abstractions;

using CampaignSaaS.Modules.Reporting.Domain.Entities;

public interface ICampaignReportRepository
{
    Task<CampaignReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CampaignReport>> GetByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<int> GetTotalReportsCountAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task AddAsync(CampaignReport report, CancellationToken cancellationToken = default);
    void Update(CampaignReport report);
}

