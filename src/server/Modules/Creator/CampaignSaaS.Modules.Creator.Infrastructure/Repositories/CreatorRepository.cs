namespace CampaignSaaS.Modules.Creator.Infrastructure.Repositories;

using CampaignSaaS.Modules.Creator.Application.Abstractions;
using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.Modules.Creator.Domain.Enums;
using CampaignSaaS.Modules.Creator.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CreatorRepository : ICreatorRepository
{
    private readonly CreatorDbContext _context;

    public CreatorRepository(CreatorDbContext context)
    {
        _context = context;
    }

    public async Task<Creator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Creators.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Creator> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        string? niche,
        PlatformType? platform,
        CreatorStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Creators
            .Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                EF.Functions.ILike(c.FullName, $"%{search}%") ||
                (c.Email != null && EF.Functions.ILike(c.Email, $"%{search}%")) ||
                (c.PhoneNumber != null && EF.Functions.ILike(c.PhoneNumber, $"%{search}%")));
        }

        if (!string.IsNullOrWhiteSpace(niche))
        {
            var nicheSearch = niche.Trim().ToLower();
            query = query.Where(c => EF.Functions.ILike(c.Niche, $"%{nicheSearch}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (platform.HasValue)
        {
            items = items.Where(c => c.SocialAccounts.Any(s => s.Platform == platform.Value)).ToList();
        }

        return (items, totalCount);
    }

    public async Task AddAsync(Creator creator, CancellationToken cancellationToken = default)
    {
        await _context.Creators.AddAsync(creator, cancellationToken);
    }

    public void Update(Creator creator)
    {
        _context.Creators.Update(creator);
    }
}
