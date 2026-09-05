namespace CampaignSaaS.Modules.Creator.Application.Abstractions;

using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.Enums;

public interface ICreatorRepository
{
    Task<Creator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Creator> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        string? niche,
        PlatformType? platform,
        CreatorStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Creator creator, CancellationToken cancellationToken = default);
    void Update(Creator creator);
}
