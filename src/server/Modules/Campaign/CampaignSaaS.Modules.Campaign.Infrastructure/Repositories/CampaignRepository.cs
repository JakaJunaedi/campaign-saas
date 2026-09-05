namespace CampaignSaaS.Modules.Campaign.Infrastructure.Repositories;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Domain.Enums;
using CampaignSaaS.Modules.Campaign.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CampaignRepository : ICampaignRepository
{
    private readonly CampaignDbContext _context;

    public CampaignRepository(CampaignDbContext context)
    {
        _context = context;
    }

    public async Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Campaign> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        Guid? clientId,
        CampaignStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Campaigns
            .Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                EF.Functions.ILike(c.Title, $"%{search}%") ||
                (c.Description != null && EF.Functions.ILike(c.Description, $"%{search}%")));
        }

        if (clientId.HasValue)
        {
            query = query.Where(c => c.ClientId == clientId.Value);
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

        return (items, totalCount);
    }

    public async Task<int> GetCreatorsCountAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignCreators.CountAsync(c => c.CampaignId == campaignId, cancellationToken);
    }

    public async Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        await _context.Campaigns.AddAsync(campaign, cancellationToken);
    }

    public void Update(Campaign campaign)
    {
        _context.Campaigns.Update(campaign);
    }
}
