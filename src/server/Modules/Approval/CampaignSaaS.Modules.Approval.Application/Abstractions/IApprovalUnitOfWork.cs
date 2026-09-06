namespace CampaignSaaS.Modules.Approval.Application.Abstractions;

public interface IApprovalUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
