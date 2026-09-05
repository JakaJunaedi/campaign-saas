namespace CampaignSaaS.Modules.Creator.Application.Abstractions;

public interface ICreatorUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
