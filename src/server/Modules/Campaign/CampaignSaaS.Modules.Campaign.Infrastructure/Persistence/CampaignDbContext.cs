namespace CampaignSaaS.Modules.Campaign.Infrastructure.Persistence;

using CampaignSaaS.Modules.Campaign.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class CampaignDbContext : DbContext
{
    private readonly ICurrentTenantContext? _tenantContext;

    public CampaignDbContext(
        DbContextOptions<CampaignDbContext> options,
        ICurrentTenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignCreator> CampaignCreators => Set<CampaignCreator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CampaignDbContext).Assembly);

        // Global multi-tenant query filter
        modelBuilder.Entity<Campaign>().HasQueryFilter(c =>
            !c.IsDeleted &&
            (_tenantContext == null ||
             _tenantContext.IsSuperAdmin ||
             (_tenantContext.OrganizationId != null && c.OrganizationId == _tenantContext.OrganizationId.Value)));

        modelBuilder.Entity<CampaignCreator>().HasQueryFilter(c =>
            _tenantContext == null ||
            _tenantContext.IsSuperAdmin ||
            (_tenantContext.OrganizationId != null && c.OrganizationId == _tenantContext.OrganizationId.Value));
    }
}
