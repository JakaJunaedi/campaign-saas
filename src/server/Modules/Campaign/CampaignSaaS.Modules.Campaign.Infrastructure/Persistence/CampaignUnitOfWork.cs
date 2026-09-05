namespace CampaignSaaS.Modules.Campaign.Infrastructure.Persistence;

using CampaignSaaS.Modules.Campaign.Application.Abstractions;

public class CampaignUnitOfWork : ICampaignUnitOfWork
{
    private readonly CampaignDbContext _context;

    public CampaignUnitOfWork(CampaignDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
