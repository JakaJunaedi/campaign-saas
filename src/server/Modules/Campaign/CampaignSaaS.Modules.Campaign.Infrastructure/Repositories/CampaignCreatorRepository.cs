namespace CampaignSaaS.Modules.Campaign.Infrastructure.Repositories;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;
using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.Modules.Campaign.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CampaignCreatorRepository : ICampaignCreatorRepository
{
    private readonly CampaignDbContext _context;

    public CampaignCreatorRepository(CampaignDbContext context)
    {
        _context = context;
    }

    public async Task<CampaignCreator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignCreators.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CampaignCreator?> GetByCampaignAndCreatorAsync(Guid campaignId, Guid creatorId, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignCreators
            .FirstOrDefaultAsync(c => c.CampaignId == campaignId && c.CreatorId == creatorId, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignCreator>> GetRosterByCampaignIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignCreators
            .Where(c => c.CampaignId == campaignId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsInCampaignAsync(Guid campaignId, Guid creatorId, CancellationToken cancellationToken = default)
    {
        return await _context.CampaignCreators
            .AnyAsync(c => c.CampaignId == campaignId && c.CreatorId == creatorId, cancellationToken);
    }

    public async Task AddAsync(CampaignCreator rosterItem, CancellationToken cancellationToken = default)
    {
        await _context.CampaignCreators.AddAsync(rosterItem, cancellationToken);
    }

    public void Update(CampaignCreator rosterItem)
    {
        _context.CampaignCreators.Update(rosterItem);
    }

    public void Remove(CampaignCreator rosterItem)
    {
        _context.CampaignCreators.Remove(rosterItem);
    }
}
