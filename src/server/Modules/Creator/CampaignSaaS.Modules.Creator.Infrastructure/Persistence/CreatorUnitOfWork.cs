namespace CampaignSaaS.Modules.Creator.Infrastructure.Persistence;

using CampaignSaaS.Modules.Creator.Application.Abstractions;

public class CreatorUnitOfWork : ICreatorUnitOfWork
{
    private readonly CreatorDbContext _context;

    public CreatorUnitOfWork(CreatorDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
