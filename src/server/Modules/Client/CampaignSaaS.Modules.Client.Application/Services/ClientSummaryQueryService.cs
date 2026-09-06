namespace CampaignSaaS.Modules.Client.Application.Services;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Contracts;

public class ClientSummaryQueryService : IClientSummaryQueryService
{
    private readonly IClientRepository _clientRepository;

    public ClientSummaryQueryService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<string?> GetClientNameAsync(Guid organizationId, Guid clientId, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, cancellationToken);
        if (client == null || client.OrganizationId != organizationId)
        {
            return null;
        }

        return client.Name;
    }

    public async Task<int> GetClientsCountAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var (_, totalCount) = await _clientRepository.GetPagedAsync(organizationId, null, 1, 1, cancellationToken);
        return totalCount;
    }
}

