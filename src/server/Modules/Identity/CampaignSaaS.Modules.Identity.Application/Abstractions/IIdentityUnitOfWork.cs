namespace CampaignSaaS.Modules.Identity.Application.Abstractions;

public interface IIdentityUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
