namespace CampaignSaaS.Modules.Campaign.Application.Abstractions;

public interface ICampaignUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
