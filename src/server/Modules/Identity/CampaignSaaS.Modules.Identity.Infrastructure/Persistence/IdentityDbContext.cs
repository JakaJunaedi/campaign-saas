namespace CampaignSaaS.Modules.Identity.Infrastructure.Persistence;

using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class IdentityDbContext : DbContext
{
    private readonly ICurrentTenantContext? _tenantContext;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        ICurrentTenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        // Global Query Filters
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted);

        modelBuilder.Entity<User>().HasQueryFilter(u =>
            _tenantContext == null ||
            _tenantContext.IsSuperAdmin ||
            (_tenantContext.OrganizationId != null && u.OrganizationId == _tenantContext.OrganizationId.Value));

        modelBuilder.Entity<RefreshToken>().HasQueryFilter(r =>
            _tenantContext == null ||
            _tenantContext.IsSuperAdmin ||
            (_tenantContext.OrganizationId != null && r.OrganizationId == _tenantContext.OrganizationId.Value));
    }
}
