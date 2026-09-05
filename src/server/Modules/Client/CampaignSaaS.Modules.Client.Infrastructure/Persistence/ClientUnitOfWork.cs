namespace CampaignSaaS.Modules.Client.Infrastructure.Persistence;

using CampaignSaaS.Modules.Client.Application.Abstractions;

public class ClientUnitOfWork : IClientUnitOfWork
{
    private readonly ClientDbContext _context;

    public ClientUnitOfWork(ClientDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
