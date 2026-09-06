namespace CampaignSaaS.Modules.Client.Contracts;

public interface IClientSummaryQueryService
{
    Task<string?> GetClientNameAsync(Guid organizationId, Guid clientId, CancellationToken cancellationToken = default);
    Task<int> GetClientsCountAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

