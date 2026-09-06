namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence;

using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public class ReportingDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public ReportingDbContext(
        DbContextOptions<ReportingDbContext> options,
        ICurrentTenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<CampaignMetric> CampaignMetrics => Set<CampaignMetric>();
    public DbSet<CampaignReport> CampaignReports => Set<CampaignReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingDbContext).Assembly);

        // Global Multi-Tenancy Query Filters
        modelBuilder.Entity<CampaignMetric>()
            .HasQueryFilter(m => _tenantContext.OrganizationId == null || m.OrganizationId == _tenantContext.OrganizationId);

        modelBuilder.Entity<CampaignReport>()
            .HasQueryFilter(r => _tenantContext.OrganizationId == null || r.OrganizationId == _tenantContext.OrganizationId);
    }
}
