namespace CampaignSaaS.Modules.Identity.Application.Abstractions;

using CampaignSaaS.Modules.Identity.Domain.Entities;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Organization> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        Domain.Enums.OrganizationStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
    void Update(Organization organization);
}

