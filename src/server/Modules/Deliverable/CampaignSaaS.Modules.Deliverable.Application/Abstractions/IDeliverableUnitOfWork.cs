namespace CampaignSaaS.Modules.Deliverable.Application.Abstractions;

public interface IDeliverableUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

