namespace CampaignSaaS.Modules.Identity.Infrastructure.Repositories;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly IdentityDbContext _context;

    public OrganizationRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Slug == slug && !o.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .IgnoreQueryFilters()
            .AnyAsync(o => o.Slug == slug, cancellationToken);
    }

    public async Task<(IReadOnlyList<Organization> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        Domain.Enums.OrganizationStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Organizations
            .IgnoreQueryFilters()
            .Where(o => !o.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(o => EF.Functions.ILike(o.Name, $"%{term}%") || EF.Functions.ILike(o.Slug, $"%{term}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _context.Organizations.AddAsync(organization, cancellationToken);
    }


    public void Update(Organization organization)
    {
        _context.Organizations.Update(organization);
    }
}
