namespace CampaignSaaS.Modules.Client.Application.Abstractions;

public interface IClientUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
