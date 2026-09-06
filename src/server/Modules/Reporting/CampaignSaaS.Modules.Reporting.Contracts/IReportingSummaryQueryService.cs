namespace CampaignSaaS.Modules.Reporting.Contracts;

public interface IReportingSummaryQueryService
{
    Task<int> GetReportsCountAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
