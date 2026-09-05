namespace CampaignSaaS.Modules.Creator.Infrastructure.Persistence;

using CampaignSaaS.Modules.Creator.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class CreatorDbContext : DbContext
{
    private readonly ICurrentTenantContext? _tenantContext;

    public CreatorDbContext(
        DbContextOptions<CreatorDbContext> options,
        ICurrentTenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Creator> Creators => Set<Creator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreatorDbContext).Assembly);

        // Global multi-tenant query filter
        modelBuilder.Entity<Creator>().HasQueryFilter(c =>
            !c.IsDeleted &&
            (_tenantContext == null ||
             _tenantContext.IsSuperAdmin ||
             (_tenantContext.OrganizationId != null && c.OrganizationId == _tenantContext.OrganizationId.Value)));
    }
}
