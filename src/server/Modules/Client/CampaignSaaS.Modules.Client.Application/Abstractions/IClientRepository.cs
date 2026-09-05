namespace CampaignSaaS.Modules.Client.Application.Abstractions;

using CampaignSaaS.Modules.Client.Domain.Entities;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid organizationId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Client> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    void Update(Client client);
}
