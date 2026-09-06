namespace CampaignSaaS.Modules.Reporting.Application.Abstractions;

public interface IReportingUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
