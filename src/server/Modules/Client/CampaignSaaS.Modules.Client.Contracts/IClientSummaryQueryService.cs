namespace CampaignSaaS.Modules.Client.Contracts;

public interface IClientSummaryQueryService
{
    Task<string?> GetClientNameAsync(Guid organizationId, Guid clientId, CancellationToken cancellationToken = default);
}
