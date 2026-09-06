namespace CampaignSaaS.Modules.Reporting.Application.Services;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts;

public class ReportingSummaryQueryService : IReportingSummaryQueryService
{
    private readonly ICampaignReportRepository _reportRepository;

    public ReportingSummaryQueryService(ICampaignReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<int> GetReportsCountAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await _reportRepository.GetTotalReportsCountAsync(organizationId, cancellationToken);
    }
}
