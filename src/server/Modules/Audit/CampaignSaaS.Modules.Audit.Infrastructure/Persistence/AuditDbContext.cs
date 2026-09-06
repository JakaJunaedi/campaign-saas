namespace CampaignSaaS.Modules.Audit.Infrastructure.Persistence;

using CampaignSaaS.Modules.Audit.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class AuditDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public AuditDbContext(
        DbContextOptions<AuditDbContext> options,
        ICurrentTenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);

        // Global Multi-Tenancy Query Filter
        modelBuilder.Entity<AuditLog>()
            .HasQueryFilter(a => _tenantContext.OrganizationId == null || a.OrganizationId == _tenantContext.OrganizationId);
    }
}
