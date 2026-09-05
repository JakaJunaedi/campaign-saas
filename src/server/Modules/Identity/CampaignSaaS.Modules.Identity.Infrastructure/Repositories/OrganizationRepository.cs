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

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await _context.Organizations.AddAsync(organization, cancellationToken);
    }

    public void Update(Organization organization)
    {
        _context.Organizations.Update(organization);
    }
}
