namespace CampaignSaaS.Modules.Client.Infrastructure.Repositories;

using CampaignSaaS.Modules.Client.Application.Abstractions;
using CampaignSaaS.Modules.Client.Domain.Entities;
using CampaignSaaS.Modules.Client.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ClientRepository : IClientRepository
{
    private readonly ClientDbContext _context;

    public ClientRepository(ClientDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = name.ToLower().Trim();
        return await _context.Clients
            .AnyAsync(c =>
                c.OrganizationId == organizationId &&
                c.Name.ToLower() == normalized &&
                (excludeId == null || c.Id != excludeId.Value),
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Client> Items, int TotalCount)> GetPagedAsync(
        Guid organizationId,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Clients
            .Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                EF.Functions.ILike(c.Name, $"%{search}%") ||
                (c.CompanyName != null && EF.Functions.ILike(c.CompanyName, $"%{search}%")));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public void Update(Client client)
    {
        _context.Clients.Update(client);
    }
}
